using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Web.Hubs
{
    public class SeatHub : Hub
    {
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, string>> _lockedSeats
            = new ConcurrentDictionary<int, ConcurrentDictionary<string, string>>();

        public async Task JoinGroup(int maLich)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, maLich.ToString());

            if (_lockedSeats.TryGetValue(maLich, out var seats))
            {
                var lockedSeatNames = seats.Keys.ToList();
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
            var seats = _lockedSeats.GetOrAdd(maLich, _ => new ConcurrentDictionary<string, string>());

            if (seats.TryAdd(seatName, Context.ConnectionId))
            {
                await Clients.GroupExcept(maLich.ToString(), Context.ConnectionId).SendAsync("SeatLocked", seatName);
            }
        }

        public async Task UnlockSeat(int maLich, string seatName)
        {
            if (_lockedSeats.TryGetValue(maLich, out var seats))
            {
                if (seats.TryGetValue(seatName, out var ownerId) && ownerId == Context.ConnectionId)
                {
                    if (seats.TryRemove(seatName, out _))
                    {
                        await Clients.GroupExcept(maLich.ToString(), Context.ConnectionId).SendAsync("SeatUnlocked", seatName);
                    }
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var maLich in _lockedSeats.Keys)
            {
                await UnlockAllMySeats(maLich);
            }
            await base.OnDisconnectedAsync(exception);
        }

        private async Task UnlockAllMySeats(int maLich)
        {
            if (_lockedSeats.TryGetValue(maLich, out var seats))
            {
                var mySeats = seats.Where(kvp => kvp.Value == Context.ConnectionId).Select(kvp => kvp.Key).ToList();
                foreach (var seatName in mySeats)
                {
                    if (seats.TryRemove(seatName, out _))
                    {
                        await Clients.Group(maLich.ToString()).SendAsync("SeatUnlocked", seatName);
                    }
                }
            }
        }
    }
}
