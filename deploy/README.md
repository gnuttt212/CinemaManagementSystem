# Staging Deployment

The GitHub Actions workflow deploys staging over SSH to a Docker host.

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

## Image

The workflow pushes images to GitHub Container Registry:

```text
ghcr.io/<owner>/<repo>:<commit-sha>
```

The deploy job pins staging to the exact commit SHA that passed build and test.
