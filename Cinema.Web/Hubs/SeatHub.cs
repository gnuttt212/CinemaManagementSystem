using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Web.Hubs
{
    /// <summary>
    /// Real-time seat selection hub using Redis as the backing store.
    /// This allows multiple app instances to share seat lock state
    /// through the SignalR Redis backplane.
    ///
    /// Redis data structure:
    ///   Hash  "seats:{maLich}"  →  field=seatName, value=connectionId
    /// </summary>
    public class SeatHub : Hub
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<SeatHub> _logger;

        public SeatHub(IConnectionMultiplexer redis, ILogger<SeatHub> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        private static string SeatsKey(int maLich) => $"seats:{maLich}";

        public async Task JoinGroup(int maLich)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, maLich.ToString());

            // Load all currently locked seats from Redis
            var db = _redis.GetDatabase();
            var entries = await db.HashGetAllAsync(SeatsKey(maLich));
            if (entries.Length > 0)
            {
                var lockedSeatNames = entries.Select(e => e.Name.ToString()).ToList();
                await Clients.Caller.SendAsync("LoadLockedSeats", lockedSeatNames);
            }
        }

        public async Task LeaveGroup(int maLich)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, maLich.ToString());
            await UnlockAllMySeats(maLich);
        }

        public async Task LockSeat(int maLich, string seatName)
        {
            var db = _redis.GetDatabase();

            // HSETNX is atomic: set only if field does not exist
            bool wasSet = await db.HashSetAsync(
                SeatsKey(maLich),
                seatName,
                Context.ConnectionId,
                When.NotExists);

            if (wasSet)
            {
                _logger.LogDebug("Seat locked: {SeatName} for schedule {MaLich} by {ConnectionId}",
                    seatName, maLich, Context.ConnectionId);

                await Clients.GroupExcept(maLich.ToString(), Context.ConnectionId)
                    .SendAsync("SeatLocked", seatName);
            }
        }

        public async Task UnlockSeat(int maLich, string seatName)
        {
            var db = _redis.GetDatabase();

            // Only unlock if the seat is owned by this connection
            var ownerId = await db.HashGetAsync(SeatsKey(maLich), seatName);
            if (ownerId.HasValue && ownerId.ToString() == Context.ConnectionId)
            {
                await db.HashDeleteAsync(SeatsKey(maLich), seatName);

                _logger.LogDebug("Seat unlocked: {SeatName} for schedule {MaLich} by {ConnectionId}",
                    seatName, maLich, Context.ConnectionId);

                await Clients.GroupExcept(maLich.ToString(), Context.ConnectionId)
                    .SendAsync("SeatUnlocked", seatName);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Scan all seat hashes to find and release seats owned by this connection.
            // Use SCAN to find all "seats:*" keys without blocking Redis.
            var db = _redis.GetDatabase();
            var server = _redis.GetServers().FirstOrDefault();

            if (server != null)
            {
                await foreach (var key in server.KeysAsync(pattern: "seats:*"))
                {
                    var keyStr = key.ToString();
                    var maLichStr = keyStr.Replace("seats:", "");

                    if (int.TryParse(maLichStr, out int maLich))
                    {
                        await UnlockAllMySeats(maLich);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        private async Task UnlockAllMySeats(int maLich)
        {
            var db = _redis.GetDatabase();
            var entries = await db.HashGetAllAsync(SeatsKey(maLich));

            var mySeats = entries
                .Where(e => e.Value.ToString() == Context.ConnectionId)
                .Select(e => e.Name.ToString())
                .ToList();

            foreach (var seatName in mySeats)
            {
                await db.HashDeleteAsync(SeatsKey(maLich), seatName);

                _logger.LogDebug("Seat auto-unlocked on disconnect: {SeatName} for schedule {MaLich}",
                    seatName, maLich);

                await Clients.Group(maLich.ToString()).SendAsync("SeatUnlocked", seatName);
            }
        }
    }
}

