# Staging Deployment

The GitHub Actions workflow deploys staging over SSH to a Docker host.

For the full pipeline behavior, image tagging, rollback, and troubleshooting flow, see:

```text
docs/ci-cd.md
```

For the complete secret and variable matrix, see:

```text
docs/environment-and-secrets.md
```

## Required GitHub Environment

Create a GitHub environment named `staging`.

Add these environment secrets:

- `STAGING_HOST`: staging server host or IP.
- `STAGING_USER`: SSH user on the staging server.
- `STAGING_SSH_KEY`: private SSH key for that user.
- `STAGING_CONNECTION_STRING`: staging SQL Server connection string.
- `STAGING_GOOGLE_CLIENT_ID`: Google OAuth client ID for staging.
- `STAGING_GOOGLE_CLIENT_SECRET`: Google OAuth client secret for staging.
- `STAGING_VNPAY_TMN_CODE`: VNPay terminal code for staging.
- `STAGING_VNPAY_HASH_SECRET`: VNPay hash secret for staging.
- `STAGING_VNPAY_BASE_URL`: VNPay base URL.
- `STAGING_VNPAY_RETURN_URL`: public staging VNPay return URL.

Optional environment secrets:

- `STAGING_PORT`: SSH port. Defaults to `22`.

Optional environment variables:

- `STAGING_APP_DIR`: remote app directory. Defaults to `/opt/cinema-management-system`.
- `STAGING_WEB_HOST_PORT`: public HTTP port on the staging host. Defaults to `8080`.
- `STAGING_ASPNETCORE_ENVIRONMENT`: defaults to `Staging`.
- `STAGING_HEALTH_URL`: if set, the workflow runs an external health check after deploy.

## Server Requirements

The staging server needs:

- Docker Engine.
- Docker Compose plugin.
- Network access to `ghcr.io`.
- Access to the staging SQL Server from the container.

The workflow copies `deploy/docker-compose.staging.yml` to the server as:

```text
/opt/cinema-management-system/docker-compose.yml
```

It also writes:

```text
/opt/cinema-management-system/.env
```

The staging compose file runs only the web container. SQL Server should be a staging database outside this compose file.

## Staging Dependencies

`deploy/docker-compose.staging.yml` is intentionally minimal. It starts only `cinema-web`.

If the current branch enables Redis, RabbitMQ, MongoDB, or MinIO in `Program.cs`, staging must provide these services separately, or you must extend the staging compose file before deploying that branch.

Recommended staging dependency model:

| Dependency | Recommended staging option |
|---|---|
| SQL Server | Managed SQL Server, SQL Server VM, or a separate compose stack with persistent volume. |
| Redis | Managed Redis or Docker Redis on the staging host. |
| RabbitMQ | Docker RabbitMQ with management plugin or managed broker. |
| MinIO/S3 | MinIO on staging host or S3-compatible object storage. |
| MongoDB | Managed MongoDB or Docker MongoDB if review/catalog document storage is enabled. |

The application container must be able to reach those services from inside Docker.

## Image

The workflow pushes images to GitHub Container Registry:

```text
ghcr.io/<owner>/<repo>:<commit-sha>
```

The deploy job pins staging to the exact commit SHA that passed build and test.

## Production Deployment from GitHub Actions

The workflow also contains a manual `deploy-production` job. It runs only through `workflow_dispatch` and targets the GitHub environment named `production`.

Production secrets and variables are documented in:

```text
docs/environment-and-secrets.md
```

Keep the `production` environment protected with required reviewers.
