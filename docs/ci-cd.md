# CI/CD Pipeline

The main workflow is `.github/workflows/ci-cd.yml`.

## Pipeline Goals

- Build the .NET solution on every PR and push.
- Run the xUnit test project.
- Build the Docker image from the root `Dockerfile`.
- Push versioned images to GitHub Container Registry (GHCR).
- Deploy staging automatically from `main` or manually through `workflow_dispatch`.
- Deploy production manually through `workflow_dispatch` with the `production` GitHub environment.

## Triggers

| Trigger | Build/Test | Docker Build | Push Image | Deploy Staging | Deploy Production |
|---|---:|---:|---:|---:|---:|
| Pull request to `main` | Yes | Yes | No | No | No |
| Push to `develop` | Yes | Yes | Yes | No | No |
| Push to `main` | Yes | Yes | Yes | Yes | No |
| Manual `workflow_dispatch` | Yes | Yes | Yes | Yes | Yes, if approved/run |

## Jobs

### `build-test`

Runs:

```bash
dotnet restore CinemaManagementSystem.sln
dotnet build CinemaManagementSystem.sln --configuration Release --no-restore
dotnet test Cinema.Tests/Cinema.Tests.csproj --configuration Release --no-build
```

The job uploads test result artifacts from `TestResults`.

### `docker-image`

Builds the app image from:

```text
Dockerfile
```

Push target:

```text
ghcr.io/<owner>/<repo>:<commit-sha>
```

PR builds do not push images. Pushes and manual runs do.

### `deploy-staging`

Copies `deploy/docker-compose.staging.yml` to the staging host, writes a remote `.env`, pulls the exact image SHA that passed CI, then restarts `cinema-web`.

Default remote app directory:

```text
/opt/cinema-management-system
```

### `deploy-production`

Copies production deployment files to the production host, writes `.env.prod`, pulls the exact image SHA that passed CI, then runs the production Compose stack.

Production is intentionally manual. Keep the GitHub `production` environment protected with required reviewers.

## Required GitHub Environments

Create these environments in GitHub:

- `staging`
- `production`

Use environment protection rules for production. At minimum, require manual approval.

## Required Permissions

The workflow uses:

```yaml
permissions:
  contents: read
  packages: write
```

The deploy jobs override package permission to read-only because they only pull already-published images.

## Release and Rollback

Every image is tagged by commit SHA. To roll back:

1. Find the last healthy image tag in GHCR.
2. SSH into the host.
3. Edit `.env` or `.env.prod` and set `WEB_IMAGE` to that tag.
4. Restart:

```bash
docker compose pull cinema-web
docker compose up -d cinema-web
```

For production, also verify:

```bash
curl --fail https://<domain>/healthz/ready
```

## Build Failure Checklist

If CI fails in `build-test`, check:

- Solution still includes projects that were moved or deleted during modular refactor.
- Controllers and services reference the correct module namespaces.
- `DbContext` types expose every `DbSet` still used by controllers/services.
- Required NuGet packages are restored and compatible with .NET 8.
- Tests still reference existing BUS/DAL types after module migration.

If CI fails in `docker-image`, check:

- `Dockerfile` copies every project required for restore.
- `.dockerignore` does not exclude source files needed by publish.
- Docker build can restore packages from NuGet.

If deploy fails, check:

- SSH secrets and host connectivity.
- Docker and Docker Compose plugin installed on the server.
- The server can pull from `ghcr.io`.
- Remote `.env` or `.env.prod` contains valid connection strings and service URLs.
