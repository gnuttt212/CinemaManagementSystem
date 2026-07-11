# Environment and Secrets

This document lists the environment variables and GitHub secrets used by local development, staging, and production.

## Local Docker Compose

Local compose file:

```text
docker-compose.yml
```

Local env example:

```text
.env.example
```

Common variables:

| Variable | Required | Purpose |
|---|---:|---|
| `ASPNETCORE_ENVIRONMENT` | No | Defaults to `Development`. |
| `WEB_HOST_PORT` | No | Host port for `cinema-web`; default `8080`. |
| `MSSQL_HOST_PORT` | No | Host port for SQL Server; default `14333`. |
| `MSSQL_SA_PASSWORD` | Yes | Local SQL `sa` password. |
| `GOOGLE_CLIENT_ID` | No | Google OAuth local client ID. |
| `GOOGLE_CLIENT_SECRET` | No | Google OAuth local secret. |
| `VNPAY_TMN_CODE` | No | VNPay sandbox terminal code. |
| `VNPAY_HASH_SECRET` | No | VNPay sandbox hash secret. |
| `VNPAY_BASE_URL` | No | VNPay sandbox/prod URL. |
| `VNPAY_RETURN_URL` | No | Local VNPay return URL. |

Recommended local Google redirect URI:

```text
http://localhost:8080/signin-google
```

Recommended local VNPay return URL:

```text
http://localhost:8080/HoaDon/VnPayReturn
```

## Staging GitHub Environment

Create a GitHub environment named:

```text
staging
```

Required environment secrets:

| Secret | Purpose |
|---|---|
| `STAGING_HOST` | SSH host/IP. |
| `STAGING_USER` | SSH user. |
| `STAGING_SSH_KEY` | Private key for SSH deploy. |
| `STAGING_CONNECTION_STRING` | SQL Server staging connection string. |
| `STAGING_GOOGLE_CLIENT_ID` | Google OAuth staging client ID. |
| `STAGING_GOOGLE_CLIENT_SECRET` | Google OAuth staging secret. |
| `STAGING_VNPAY_TMN_CODE` | VNPay staging/sandbox terminal code. |
| `STAGING_VNPAY_HASH_SECRET` | VNPay staging/sandbox hash secret. |
| `STAGING_VNPAY_BASE_URL` | VNPay base URL. |
| `STAGING_VNPAY_RETURN_URL` | Public staging return URL. |

Optional environment secrets:

| Secret | Default | Purpose |
|---|---|---|
| `STAGING_PORT` | `22` | SSH port. |

Optional environment variables:

| Variable | Default | Purpose |
|---|---|---|
| `STAGING_APP_DIR` | `/opt/cinema-management-system` | Remote deployment directory. |
| `STAGING_WEB_HOST_PORT` | `8080` | Host port exposed by staging compose. |
| `STAGING_ASPNETCORE_ENVIRONMENT` | `Staging` | ASP.NET Core environment. |
| `STAGING_HEALTH_URL` | empty | External health check URL after deploy. |

### Staging Dependency Policy

The current staging compose file runs only `cinema-web`. Therefore staging must provide these dependencies externally or through a separate compose override:

- SQL Server
- Redis, if the current app branch requires distributed session/cache/SignalR backplane
- RabbitMQ, if background messaging is enabled
- MinIO/S3, if poster object storage is enabled
- MongoDB, if catalog reviews are enabled

If staging should be a self-contained environment, extend `deploy/docker-compose.staging.yml` with the same dependency services used by production.

## Production GitHub Environment

Create a GitHub environment named:

```text
production
```

Recommended: enable required reviewers before deploy.

Required environment secrets:

| Secret | Purpose |
|---|---|
| `PROD_HOST` | SSH host/IP. |
| `PROD_USER` | SSH user. |
| `PROD_SSH_KEY` | Private key for SSH deploy. |
| `PROD_CONNECTION_STRING` | Production SQL Server connection string. |
| `PROD_MSSQL_SA_PASSWORD` | SQL Server `sa` password used by compose. |
| `PROD_GOOGLE_CLIENT_ID` | Google OAuth production client ID. |
| `PROD_GOOGLE_CLIENT_SECRET` | Google OAuth production secret. |
| `PROD_VNPAY_TMN_CODE` | VNPay production terminal code. |
| `PROD_VNPAY_HASH_SECRET` | VNPay production hash secret. |
| `PROD_VNPAY_BASE_URL` | VNPay production URL. |
| `PROD_VNPAY_RETURN_URL` | Public HTTPS production return URL. |

Optional environment secrets:

| Secret | Default | Purpose |
|---|---|---|
| `PROD_PORT` | `22` | SSH port. |

Optional environment variables:

| Variable | Default | Purpose |
|---|---|---|
| `PROD_APP_DIR` | `/opt/cinema-management-system` | Remote deployment directory. |
| `PROD_DOMAIN` | `cinema.yourdomain.com` | Production domain. |
| `PROD_CERTBOT_EMAIL` | empty | Email for Let's Encrypt notifications. |

## Production `.env.prod`

Production compose also supports these variables in `deploy/.env.prod.example`:

| Variable | Purpose |
|---|---|
| `WEB_IMAGE` | GHCR image to run. |
| `MSSQL_PID` | SQL Server edition. |
| `ConnectionStrings__Redis` | Redis connection string. |
| `ConnectionStrings__RabbitMQ` | RabbitMQ connection string, for example `amqp://user:password@cinema-rabbitmq:5672/`. |
| `ConnectionStrings__MongoDB` | MongoDB connection string if MongoDB is enabled. |
| `MinIO__Endpoint` | MinIO endpoint inside Docker network. |
| `MinIO__AccessKey` | MinIO access key. |
| `MinIO__SecretKey` | MinIO secret key. |
| `MinIO__BucketName` | Poster bucket. |
| `MinIO__PublicBaseUrl` | Public poster URL prefix. |
| `RABBITMQ_USER` | RabbitMQ default user for compose. |
| `RABBITMQ_PASSWORD` | RabbitMQ default password for compose. |
| `BACKUP_RETENTION_DAYS` | Backup retention. |
| `GRAFANA_ADMIN_USER` | Grafana admin user. |
| `GRAFANA_ADMIN_PASSWORD` | Grafana admin password. |
| `GRAFANA_ROOT_URL` | Public Grafana URL. |

## Secret Hygiene

- Do not commit `.env`, `.env.prod`, private keys, Google secrets, VNPay secrets, or SQL passwords.
- Rotate any secret that has appeared in Git history or screenshots.
- Use different Google OAuth and VNPay credentials for local, staging, and production.
- VNPay return URLs must match the actual public environment URL.
- Production SQL backups must not be stored only on the same host forever; copy them to external storage.
