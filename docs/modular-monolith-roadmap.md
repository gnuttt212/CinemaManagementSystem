# Modular Monolith and Messaging Roadmap

This document captures the planned migration path from the current monolith to a modular monolith, then to selected microservices.

## Current Direction

The system should move in two stages:

1. Modular Monolith with asynchronous messaging.
2. Microservices only for modules that need independent scale or deployment.

Do not split everything at once. The first goal is clear module boundaries inside one process.

## Stage 1: Modular Monolith

### Target Modules

| Module | Responsibility | Data Ownership |
|---|---|---|
| `IdentityModule` | Login, Google OAuth, customers, staff, roles, sessions/auth decisions. | `KhachHang`, `NhanVien`, auth-related state. |
| `CatalogModule` | Movies, rooms, seats metadata, food/drinks, promotions, reviews. | `Phim`, `PhongChieu`, `Ghe`, `DoAn`, `KhuyenMai`, review documents. |
| `BookingModule` | Seat locking, carts, invoices, VNPay callback, booking lifecycle. | `HoaDon`, `ChiTietHoaDon`, `ChiTietDoAn`, booking transactions. |
| `NotificationModule` | E-ticket email, QR delivery, retryable user notifications. | Notification outbox/attempt log. |
| `ReportingModule` | Revenue dashboards and exports. | Read-only projections/views. |

### Rules

- Modules run in the same ASP.NET Core process.
- Controllers can call only the public application service interfaces of a module.
- A module should not query another module's tables directly.
- Shared code should be minimal: primitive DTOs, integration events, and cross-cutting abstractions.
- Each module owns its DbContext or repository boundary.
- Cross-module workflows should use domain/integration events, not direct database joins.

### Suggested Folder Shape

```text
Cinema.Web/
  Modules/
    Identity/
      Contracts/
      Data/
      Entities/
      Services/
    Catalog/
      Contracts/
      Data/
      Entities/
      Services/
    Booking/
      Contracts/
      Data/
      Entities/
      Services/
      Events/
    Notification/
      Consumers/
      Events/
      Services/
    Reporting/
      Queries/
      Services/
```

## Stage 1 Messaging

Use RabbitMQ for asynchronous work that should not block HTTP requests.

Recommended first use cases:

- Send E-Ticket email after payment success.
- Retry failed email delivery.
- Handle VNPay refund workflows asynchronously.
- Publish booking lifecycle events for reporting.

### Event Examples

```text
BookingConfirmed
  BookingId
  InvoiceId
  CustomerId
  Email
  MovieTitle
  SeatCodes
  TotalAmount
  OccurredAtUtc

TicketEmailRequested
  BookingId
  InvoiceId
  RecipientEmail
  QrPayload
  OccurredAtUtc

VnPayRefundRequested
  InvoiceId
  TransactionId
  Amount
  Reason
  OccurredAtUtc
```

### Worker Responsibilities

| Worker | Queue | Responsibility |
|---|---|---|
| `TicketEmailWorker` | `ticket-email` | Render and send E-Ticket email, retry failures, log attempts. |
| `PaymentRefundWorker` | `vnpay-refund` | Process refund request, record external response, publish result event. |
| `OutboxDispatcher` | `outbox` or DB polling | Read unpublished outbox messages and publish to RabbitMQ. |

## Transactional Outbox

Use the outbox whenever a database transaction must also publish an event.

Example flow:

1. VNPay callback confirms payment.
2. Booking transaction updates `HoaDon`, seats, and ticket state.
3. Same SQL transaction inserts `BookingConfirmed` into `OutboxMessages`.
4. HTTP request returns quickly.
5. `OutboxDispatcher` publishes the event to RabbitMQ.
6. `TicketEmailWorker` consumes and sends the E-Ticket.

Required table shape:

```text
OutboxMessages
  Id
  Type
  PayloadJson
  OccurredAtUtc
  ProcessedAtUtc
  RetryCount
  LastError
```

## Stage 2: Selective Microservices

Only extract a module when it has a clear reason:

- It needs independent scaling.
- It has a different release cadence.
- It has a strong data ownership boundary.
- It creates operational value greater than the complexity cost.

### First Candidate: Booking Service

Booking is the strongest candidate because it owns:

- Seat locking.
- Booking lifecycle.
- Payment callback consistency.
- High-concurrency workflows.

When extracted:

- `BookingService` owns booking database tables.
- Other services consume booking events instead of reading booking tables.
- Public access goes through an API Gateway.

## API Gateway

Use YARP as the first API Gateway because it fits the .NET ecosystem.

Responsibilities:

- Route `/booking/*` to Booking Service.
- Route `/catalog/*` to Catalog/Web module.
- Centralize TLS/proxy rules.
- Keep authentication and authorization policies consistent.

## Saga

Use saga orchestration only for workflows that cross service boundaries and cannot be completed in one local transaction.

Candidate saga:

```text
BookingPaymentSaga
  Reserve seats
  Create invoice
  Redirect/pay through VNPay
  Confirm payment callback
  Commit booking
  Publish ticket email request
  If payment/refund fails, release seats and mark invoice failed
```

## Migration Checklist

1. Make current solution compile and tests pass after module folder move.
2. Define module contracts before moving more logic.
3. Move business services into module namespaces.
4. Move DbContexts/entities into owning modules.
5. Replace direct cross-module DB queries with service contracts or read models.
6. Add RabbitMQ to local and production compose files.
7. Add outbox table and dispatcher.
8. Move E-Ticket email into background worker.
9. Move refund handling into background worker.
10. Add integration tests for booking/payment/event flow.
11. Add YARP only when at least one service is extracted.
12. Extract Booking Service last, after the modular monolith boundary is stable.

## Risks

- Moving entities before controllers/services are updated will break build.
- Multiple DbContexts can accidentally duplicate entity ownership.
- RabbitMQ without an outbox can lose events when SQL commits but publish fails.
- Email/refund workers must be idempotent because messages can be delivered more than once.
- Reporting queries may need read models after module boundaries are enforced.
