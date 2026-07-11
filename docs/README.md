# Documentation Index

This folder contains architecture, DevOps, and operations documentation for the Cinema Management System.

## Start Here

- [Docker Development Setup](../DOCKER.md): local Docker Compose setup for the monolith and SQL Server dev database.
- [CI/CD Pipeline](ci-cd.md): GitHub Actions build, test, image publish, staging deploy, and production deploy flow.
- [Environment and Secrets](environment-and-secrets.md): required local, staging, production, and GitHub Actions variables.
- [Staging Deployment](../deploy/README.md): staging Docker host deployment over SSH.
- [Production Deployment](../deploy/PRODUCTION.md): production Docker Compose deployment with nginx, SSL, backup, monitoring, and rollback notes.

## Architecture

- [C4 Model](c4-model.md): system context, container, and component views.
- [Modular Monolith and Messaging Roadmap](modular-monolith-roadmap.md): staged plan for module boundaries, RabbitMQ/background workers, outbox, and future microservices.

## Architecture Decision Records

- [ADR-001: Record Architecture Decisions](adr/001-record-architecture-decisions.md)
- [ADR-002: 3-Tier Architecture](adr/002-3-tier-architecture.md)
- [ADR-003: Redis Distributed State](adr/003-redis-distributed-state.md)
- [ADR-004: MinIO Object Storage](adr/004-minio-object-storage.md)
- [ADR-005: Docker Compose Deployment](adr/005-docker-compose-deployment.md)
- [ADR-006: Modular Monolith and Messaging](adr/006-modular-monolith-and-messaging.md)

## Current Operational Notes

- Local development can run with the root `docker-compose.yml`.
- Staging uses `deploy/docker-compose.staging.yml` and expects stateful dependencies to be supplied by the staging environment unless explicitly added to that compose file.
- Production uses `deploy/docker-compose.prod.yml`, which includes SQL Server, Redis, RabbitMQ, MinIO, nginx, certbot, backup, Prometheus, and Grafana.
- SQL Server production data should be backed up and restore-tested before every major deployment.
