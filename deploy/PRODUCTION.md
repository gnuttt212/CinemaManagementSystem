# Production Deployment Guide

This guide covers deploying the Cinema Management System to a production Linux server using Docker Compose, nginx, and Let's Encrypt SSL.

## Architecture Overview

```
Internet
   │
   ▼
┌──────────────┐
│ nginx (:443) │──── SSL termination, rate limiting, static caching
└──────┬───────┘
       │ proxy_pass :8080
       ▼
┌──────────────┐     ┌──────────────┐
│  cinema-web  │────▶│  cinema-db   │ SQL Server 2022
│  ASP.NET 8.0 │     │  (internal)  │
└──────────────┘     └──────────────┘
                           ▲
                     ┌─────┴──────┐
                     │cinema-backup│ Daily 2 AM
                     └────────────┘
```

All services communicate over an isolated Docker bridge network (`cinema-net`).
Only nginx exposes ports 80 and 443 to the host.

## Prerequisites

- **Linux server**: Ubuntu 22.04+ recommended
- **Docker Engine**: 24.0+ with Docker Compose v2 (`docker compose` command)
- **Domain name**: DNS A record pointing to the server's public IP
- **Firewall**: Ports 80, 443, and 22 (SSH) open; all others blocked
- **RAM**: Minimum 4 GB (SQL Server requirement)
- **Disk**: At least 20 GB free for database, backups, and images

## Step-by-Step Deployment

### 1. Clone Repository

```bash
git clone https://github.com/your-org/CinemaManagementSystem.git
cd CinemaManagementSystem/deploy
```

### 2. Configure Environment

```bash
cp .env.prod.example .env.prod
```

Edit `.env.prod` and fill in **every** value:

| Variable | Description |
|---|---|
| `WEB_IMAGE` | Docker image for the web app (e.g., `ghcr.io/your-org/cinemamanagement:v1.0.0`) |
| `MSSQL_SA_PASSWORD` | SQL Server SA password — must be 16+ chars with mixed case, digits, symbols |
| `MSSQL_PID` | SQL Server edition: `Express`, `Developer`, `Standard`, or `Enterprise` |
| `ConnectionStrings__DefaultConnection` | ADO.NET connection string — password must match `MSSQL_SA_PASSWORD` |
| `Authentication__Google__ClientId` | Google OAuth 2.0 client ID |
| `Authentication__Google__ClientSecret` | Google OAuth 2.0 client secret |
| `VnPay__TmnCode` | VNPay merchant terminal code |
| `VnPay__HashSecret` | VNPay HMAC secret key |
| `VnPay__ReturnUrl` | VNPay payment return URL (must use your production domain) |
| `DOMAIN_NAME` | Your production domain (e.g., `cinema.yourdomain.com`) |
| `CERTBOT_EMAIL` | Email for Let's Encrypt certificate notifications |
| `BACKUP_RETENTION_DAYS` | Days to keep old backup files (default: 7) |

> [!IMPORTANT]
> The `MSSQL_SA_PASSWORD` value must appear identically in both the `MSSQL_SA_PASSWORD` variable and the `ConnectionStrings__DefaultConnection` password field.

### 3. Initial SSL Certificate

The first deployment requires a two-step process because nginx needs certificates to start, but certbot needs nginx to verify the domain.

**Step 3a: Start nginx in HTTP-only mode**

Temporarily comment out the HTTPS server block in `nginx/conf.d/default.conf` (or create a minimal HTTP-only config), then:

```bash
docker compose -f docker-compose.prod.yml up -d cinema-nginx
```

**Step 3b: Obtain certificate**

```bash
docker compose -f docker-compose.prod.yml run --rm certbot certonly \
  --webroot -w /var/www/certbot \
  -d cinema.yourdomain.com \
  --email admin@yourdomain.com \
  --agree-tos --no-eff-email
```

**Step 3c: Restore full nginx config and restart**

Uncomment the HTTPS server block, then:

```bash
docker compose -f docker-compose.prod.yml restart cinema-nginx
```

### 4. Start All Services

```bash
docker compose -f docker-compose.prod.yml up -d
```

Monitor startup:

```bash
docker compose -f docker-compose.prod.yml logs -f
```

Wait until you see:
- `SQL Server is ready.` from `cinema-db-init`
- `Database initialized.` (or `Database already initialized.`)
- `cinema-web` listening on port 8080

### 5. Verify Deployment

**Health check:**

```bash
curl -s https://cinema.yourdomain.com/healthz
# Expected: Healthy

curl -s https://cinema.yourdomain.com/healthz/ready
# Expected: Healthy (includes DB connectivity check)
```

**SSL verification:**

```bash
curl -I https://cinema.yourdomain.com
# Verify: HTTP/2 200, Strict-Transport-Security header present
```

**Security headers:**

```bash
curl -sI https://cinema.yourdomain.com | grep -iE '(x-frame|x-content|x-xss|referrer-policy|strict-transport)'
# Expected:
# X-Frame-Options: DENY
# X-Content-Type-Options: nosniff
# X-XSS-Protection: 1; mode=block
# Referrer-Policy: strict-origin-when-cross-origin
# Strict-Transport-Security: max-age=31536000; includeSubDomains
```

**WebSocket (SignalR) test:**

Open the browser developer tools Network tab, navigate to a seat selection page, and verify the WebSocket connection to `/seatHub` is established.

---

## Maintenance

### SSL Certificate Renewal

The `certbot` container runs an automatic renewal loop every 12 hours. Certbot only renews certificates within 30 days of expiry.

**Manual renewal:**

```bash
docker compose -f docker-compose.prod.yml run --rm certbot renew
docker compose -f docker-compose.prod.yml exec cinema-nginx nginx -s reload
```

**Recommended cron job** to reload nginx after renewal (on the host):

```bash
# Add to crontab: crontab -e
0 0 */15 * * cd /path/to/deploy && docker compose -f docker-compose.prod.yml exec cinema-nginx nginx -s reload >/dev/null 2>&1
```

### Database Backup

**Automatic:** The `cinema-backup` container runs a backup daily at 2:00 AM server time.

**Manual backup:**

```bash
docker compose -f docker-compose.prod.yml exec cinema-backup /scripts/backup-db.sh
```

**Restore from backup:**

```bash
# List available backups
docker compose -f docker-compose.prod.yml exec cinema-backup ls -lh /backups/

# Restore (interactive confirmation)
docker compose -f docker-compose.prod.yml exec cinema-backup /scripts/restore-db.sh /backups/QuanLyRapPhim_20260709_020000.bak

# Restore (skip confirmation — for scripts)
docker compose -f docker-compose.prod.yml exec cinema-backup /scripts/restore-db.sh /backups/QuanLyRapPhim_20260709_020000.bak -y
```

> [!WARNING]
> Restoring a database will **replace all current data** with the backup contents. Always verify you are restoring the correct backup file.

**Backup file location:**

Backups are stored in the `cinema-backup-data` Docker volume. To find the mount point:

```bash
docker volume inspect cinema-backup-data --format '{{ .Mountpoint }}'
```

### Viewing Logs

**Application logs:**

```bash
docker compose -f docker-compose.prod.yml logs cinema-web --tail 100 -f
```

**Structured log files** (if configured for file logging):

```bash
# Find the volume mount point
docker volume inspect cinema-logs --format '{{ .Mountpoint }}'
# Then browse the log files
ls -la $(docker volume inspect cinema-logs --format '{{ .Mountpoint }}')
```

**Nginx access/error logs:**

```bash
docker compose -f docker-compose.prod.yml logs cinema-nginx --tail 100 -f
```

**Database logs:**

```bash
docker compose -f docker-compose.prod.yml logs cinema-db --tail 100 -f
```

### Updating the Application

```bash
# 1. Pull the new image
docker compose -f docker-compose.prod.yml pull cinema-web

# 2. Recreate only the web container (zero-downtime with health checks)
docker compose -f docker-compose.prod.yml up -d cinema-web

# 3. Verify
curl -s https://cinema.yourdomain.com/healthz
```

### Scaling Considerations

- **Horizontal scaling:** Run multiple `cinema-web` instances behind nginx by adding replicas and updating the upstream block in `default.conf`
- **Database:** Consider SQL Server Standard or Enterprise edition for production workloads exceeding Express edition limits (10 GB database size, 1 GB RAM)
- **CDN:** Place a CDN (e.g., Cloudflare) in front of nginx for static assets and DDoS protection

---

## Troubleshooting

### Port 80/443 Already in Use

```bash
# Find what's using the ports
sudo ss -tlnp | grep -E ':80|:443'
# Stop the conflicting service (e.g., Apache)
sudo systemctl stop apache2 && sudo systemctl disable apache2
```

### SSL Certificate Not Found (First Deployment)

If nginx fails to start because SSL certificates don't exist yet:

1. Temporarily use an HTTP-only nginx config (see Step 3a above)
2. Obtain the certificate with certbot
3. Restore the full nginx config with SSL

### Database Connection Timeout

```bash
# Check if SQL Server is running
docker compose -f docker-compose.prod.yml logs cinema-db --tail 20

# Verify connectivity from the web container
docker compose -f docker-compose.prod.yml exec cinema-web \
  sh -c 'apt-get update && apt-get install -y iputils-ping && ping -c 3 cinema-db'

# Common causes:
# - MSSQL_SA_PASSWORD doesn't match ConnectionStrings password
# - SQL Server hasn't finished starting (wait 30-60s on first boot)
# - Insufficient RAM (SQL Server needs at least 2 GB)
```

### WebSocket / SignalR Not Connecting

```bash
# Verify the /seatHub location block is proxying correctly
docker compose -f docker-compose.prod.yml exec cinema-nginx nginx -T | grep -A 10 seatHub

# Check nginx error log for WebSocket upgrade issues
docker compose -f docker-compose.prod.yml logs cinema-nginx | grep -i websocket

# Common causes:
# - Missing Upgrade/Connection headers in nginx config
# - proxy_read_timeout too low (should be 86400s)
# - Client behind a corporate proxy that strips WebSocket headers
```

### Container Won't Start

```bash
# Check container status and exit codes
docker compose -f docker-compose.prod.yml ps -a

# View logs for the failing container
docker compose -f docker-compose.prod.yml logs <service-name> --tail 50

# Inspect container details
docker inspect <container-name> | jq '.[0].State'
```

---

## Security Checklist

Before going live, verify every item:

- [ ] All secrets are in `.env.prod` (not committed to source control)
- [ ] `.env.prod` is in `.gitignore`
- [ ] `MSSQL_SA_PASSWORD` is strong (16+ characters, mixed case, digits, symbols)
- [ ] Google OAuth credentials are production credentials (not development/leaked ones)
- [ ] VNPay credentials are production credentials (not sandbox/leaked ones)
- [ ] `VnPay__ReturnUrl` uses the production domain with HTTPS
- [ ] Firewall allows only ports 80, 443, and 22
- [ ] SQL Server port (1433) is **NOT** exposed to the host
- [ ] SSL certificate is obtained and auto-renewing
- [ ] HSTS header is present in responses
- [ ] Database backup is running daily and verified
- [ ] Backup restore has been tested at least once
- [ ] Application logs are being collected
- [ ] Monitoring/alerting is configured for health check endpoints
