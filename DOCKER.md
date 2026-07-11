# Docker Development Setup

This setup runs the ASP.NET Core MVC monolith and a local SQL Server Developer container.

## Services

- `cinema-web`: ASP.NET Core MVC app built from `Cinema.Web`.
- `cinema-db`: SQL Server 2022 Developer for local development.
- `cinema-db-init`: one-shot database initializer using `DatabaseScripts/CinemaManagementSystem_Full.sql`.

## Run

```bash
docker compose up --build
```

Open the app at:

```text
http://localhost:8080
```

SQL Server is exposed on the host at:

```text
localhost,14333
```

Default local SQL credentials:

```text
User: sa
Password: CinemaDev@12345
Database: QuanLyRapPhim
```

## Environment

For local overrides, copy `.env.example` to `.env` and change values as needed.

The Compose file injects these settings into the app:

- `ConnectionStrings__DefaultConnection`
- `Authentication__Google__ClientId`
- `Authentication__Google__ClientSecret`
- `VnPay__TmnCode`
- `VnPay__HashSecret`
- `VnPay__ReturnUrl`

## Database Reset

The init container skips the SQL script when the `QuanLyRapPhim` database already contains the `Phim` table.

To reset local SQL data:

```bash
docker compose down -v
docker compose up --build
```

## Notes

- Production SQL Server should stay outside this Compose setup.
- The app container serves HTTP on port `8080`; terminate HTTPS at a reverse proxy in staging/production.
- Uploaded poster files are persisted in the `cinema-poster-data` Docker volume.
- Google OAuth and VNPay require environment-specific redirect/return URLs.

## CI/CD

GitHub Actions CI/CD is defined in `.github/workflows/ci-cd.yml`.

Staging deployment files and required secrets are documented in `deploy/README.md`.

Additional docs:

- `docs/ci-cd.md`
- `docs/environment-and-secrets.md`
- `docs/modular-monolith-roadmap.md`
- `docs/README.md`

---

## Production Deployment

See [deploy/PRODUCTION.md](deploy/PRODUCTION.md) for the full production deployment guide.

### Quick Start

```bash
# 1. Copy and fill environment file
cp deploy/.env.prod.example deploy/.env.prod

# 2. Obtain SSL certificate (first time)
docker compose -f deploy/docker-compose.prod.yml run --rm certbot certonly \
  --webroot -w /var/www/certbot \
  -d cinema.yourdomain.com \
  --email admin@yourdomain.com --agree-tos --no-eff-email

# 3. Start all services
cd deploy
docker compose -f docker-compose.prod.yml up -d

# 4. Check health
curl https://cinema.yourdomain.com/healthz
```

### Manual Backup

```bash
docker compose -f deploy/docker-compose.prod.yml exec cinema-backup /scripts/backup-db.sh
```

### Restore from Backup

```bash
docker compose -f deploy/docker-compose.prod.yml exec cinema-backup /scripts/restore-db.sh /backups/QuanLyRapPhim_20260709.bak -y
```
