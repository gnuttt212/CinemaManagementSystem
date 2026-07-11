# ADR-006: Modular Monolith and Asynchronous Messaging

Date: 2026-07-11

## Status

Proposed / In progress

## Context

The application started as a 3-layer ASP.NET Core MVC monolith. This is simple and productive, but the booking/payment/seat-locking workflow is becoming more complex and more load-sensitive than the rest of the application.

The project also has asynchronous work that should not block the user request path:

- Sending E-Ticket email after successful payment.
- Retrying failed email delivery.
- Processing VNPay refund workflows.
- Publishing booking events for reporting and future services.

Splitting directly into microservices would add operational complexity before module boundaries are stable.

## Decision

Adopt a modular monolith first.

- Keep one deployable ASP.NET Core process.
- Organize code into modules such as Identity, Catalog, Booking, Notification, and Reporting.
- Give each module clear contracts and data ownership.
- Avoid direct database access across module boundaries.
- Use RabbitMQ for asynchronous background work.
- Use the transactional outbox pattern for workflows that must update SQL Server and publish events reliably.

Microservice extraction is deferred until module boundaries are stable. Booking is the first candidate because it owns high-concurrency seat locking and payment consistency.

## Consequences

Positive:

- Safer transition than a big-bang microservices migration.
- Clearer module ownership.
- Booking/payment request path becomes faster because email/refund work moves to workers.
- RabbitMQ and outbox prepare the system for future microservices.

Negative:

- More project structure and dependency rules to enforce.
- Multiple DbContexts and module contracts can make refactoring harder at first.
- RabbitMQ adds infrastructure and operational monitoring needs.
- Workers must be idempotent and retry-safe.

## Follow-up Work

- Finish module refactor until `dotnet build` and tests are green.
- Add outbox schema and dispatcher.
- Add RabbitMQ health checks and alerts.
- Add integration tests for booking/payment/event publishing.
- Document event contracts once finalized.
