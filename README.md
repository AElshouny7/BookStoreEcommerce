# BookStoreEcommerce API

An end-to-end ASP.NET Core 8.0 backend for a Book Store e-commerce platform. It provides product & category management, user authentication & authorization (JWT + role-based + custom "Self" policy), order & order items handling, caching (Redis), background jobs (Hangfire), and asynchronous messaging (RabbitMQ via MassTransit) to re-engage inactive users.

## Table of Contents

1. Overview
2. Features
3. Architecture & Design
4. Tech Stack
5. Prerequisites
6. Quick Start (Local)
7. Quick Start (Docker Compose)
8. Configuration
9. Database & Migrations
10. Authentication & Authorization
11. Caching Strategy
12. Background Jobs & Messaging
13. API Endpoints (Summary)
14. Challenges & Solutions
15. Troubleshooting & FAQ
16. Contributing & Next Steps
17. License

---

## 1. Overview

This service exposes RESTful endpoints to manage users, products, categories, orders, and order items for a typical bookstore scenario. It also includes a job that periodically identifies inactive users and publishes a message that can trigger re-engagement workflows.

## 2. Features

- JWT Authentication with access token issuance.
- Role-based authorization (Admin) + custom policy (Self) to restrict certain actions to the resource owner.
- Product & Category CRUD (Admin creation/update/delete; public read).
- Orders & Order Items management (create, update status, filter by user/product/order).
- Redis caching with key namespacing and TTL configuration.
- Hangfire scheduled recurring job (daily) for inactive user re-engagement.
- MassTransit + RabbitMQ integration for event-driven communication.
- PostgreSQL persistence using EF Core with code-first migrations.
- AutoMapper DTO projections.
- CORS configuration for an Angular frontend (`http://localhost:4200`).
- Swagger / OpenAPI documentation in Development environment.

## 3. Architecture & Design

Layered approach:

- Controllers: HTTP endpoint definitions & minimal orchestration.
- Services (`Services/*`): Core business logic; isolation from transport & data layers.
- Repositories (`DBContext/*Repo.cs`): Data access via EF Core `StoreDbContext`.
- DTOs (`Dtos/*`): Shape responses & requests; prevent over-posting.
- Auth (`Auth/*`): Policies, handlers, requirements; token service.
- Jobs (`Jobs/*`): Hangfire recurring job implementation.
- Messaging (`Messaging/*`): Consumer & contract for inactive user reengagement.
- Caching (`Services/Caching/*`): Abstraction atop Redis for simple TTL and namespacing.

Cross-cutting concerns:

- AutoMapper profiles in `Profiles/`.
- Middleware snippet in `Program.cs` that "touches" user last activity asynchronously.

## 4. Tech Stack

- Runtime: .NET 8.0 (ASP.NET Core Web API)
- Data: PostgreSQL 16 (EF Core 9 + Npgsql provider)
- Cache: Redis 7
- Messaging: RabbitMQ 3.13 + MassTransit 8
- Background jobs: Hangfire (PostgreSQL storage)
- Auth: JWT + custom policy handler
- Containerization: Docker & Docker Compose
- Observability: Swagger (dev), Hangfire dashboard (`/hangfire`)

## 5. Prerequisites

Ensure the following are installed on your machine:

- .NET SDK 8.0
- Docker Desktop (for containerized run)
- PowerShell 5+ (Windows default) or compatible shell
- Optional: PostgreSQL & Redis locally (if not using Docker)

## 6. Quick Start (Local)

```powershell
# Clone repository
git clone https://github.com/AElshouny7/BookStoreEcommerce.git
cd BookStoreEcommerce

# Restore dependencies
dotnet restore

# (Optional) Install EF CLI if not already available
dotnet tool install --global dotnet-ef

# Apply migrations (ensure a local PostgreSQL is running matching appsettings.json)
dotnet ef database update

# Run (Development will expose Swagger at https://localhost:7049/swagger)
dotnet run --project BookStoreEcommerce.csproj
```

Local URLs (launchSettings):

- HTTP: `http://localhost:5268`
- HTTPS: `https://localhost:7049`

## 7. Quick Start (Docker Compose)

The compose file currently includes infrastructure (Postgres, Redis, RedisInsight, RabbitMQ). The API service section is commented out. Uncomment to build & run the API in the same network.

```yaml
# In docker-compose.yml uncomment bookstore_api service then run:
```

```powershell
# Create env vars for RabbitMQ (optional if using defaults)
$Env:RABBITMQ_USER="guest"; $Env:RABBITMQ_PASS="guest"

# Start infra (and API if enabled)
docker compose up -d --build

# View logs
docker compose logs -f postgres

# Stop & clean
docker compose down
```

Infra component URLs:

- RabbitMQ Management: <http://localhost:15672> (default guest/guest)
- RedisInsight: <http://localhost:5540>
- Postgres: `localhost:5432`

If the API container is exposed (example mapping 5000:80) then Swagger: `http://localhost:5000/swagger`.

## 8. Configuration

Main config in `appsettings.json`:

- `ConnectionStrings:DefaultConnection` for EF Core.
- `Redis` section for cache configuration & instance key prefix.
- `Jwt` for issuer, audience, token duration (secret key provided via user secrets or environment variable—ensure you set `Jwt:Key`).
- `Cors:AllowedOrigins` currently includes `http://localhost:4200`.
- `RabbitMq` host/credentials.
- `Hangfire` Postgres connection & dashboard path.

Override values in `appsettings.Development.json`, environment variables, or user secrets. Example environment variable pattern:

```powershell
$Env:ConnectionStrings__DefaultConnection="Host=postgres;Database=bookstore;Username=shouny;Password=ecommercepass"
$Env:Redis__Configuration="redis:6379"
$Env:Jwt__Key="SuperSecretDevKeyChangeMe"
```

## 9. Database & Migrations

EF Core migrations reside in `Migrations/`. To add & apply a migration:

```powershell
# Add new migration
dotnet ef migrations add MeaningfulName

# Apply migrations to configured database
dotnet ef database update
```

Seed data occurs in `PrepDb.PrepPopulation(app);` during startup.

## 10. Authentication & Authorization

- Login returns an `AuthResponseDto` containing JWT and user info.
- Register creates a user and returns `UserReadDto`.
- Admin role gates product/category mutation and some order operations.
- Custom Policy `Self`: ensures the authenticated user matches route parameter `id` (see `SelfAuthorizationHandler`). Use `[Authorize(Roles="Self")]` currently for order creation; consider adjusting to `[Authorize(Policy = Policies.Self)]` (improvement opportunity).

Include `Authorization: Bearer <token>` header for protected endpoints.

## 11. Caching Strategy

Redis configured with a key prefix `bookstore:`. `CacheService` centralizes operations & TTL via `CacheTtls`. Example keys may include product lists or category lists. Default TTL set to 300 seconds.

Potential improvement: add cache invalidation hooks in mutation service methods.

## 12. Background Jobs & Messaging

- Hangfire Dashboard at `/hangfire` (dev) displays recurring job `reengage-inactive-7d`.
- Recurring job invokes `IUserActivityService` to find inactive users and publish a `InactiveCustomerReengageRequested` message.
- MassTransit consumer `InactiveCustomerReengageConsumer` processes messages (extend to send emails, etc.).

## 13. API Endpoints (Summary)

Base path: `/api`

Auth (`/api/Auth`):

- `POST /api/Auth/register` Register new user
- `POST /api/Auth/login` Login & receive JWT

Users (`/api/Users`) [Authenticated]

- `GET /api/Users` (Admin) List users
- `GET /api/Users/{id}` Get user
- `PUT /api/Users/{id}` Update user
- `DELETE /api/Users/{id}` Delete user

Categories (`/api/Categories`):

- `GET /api/Categories` List categories
- `GET /api/Categories/{id}` Get category
- `POST /api/Categories` (Admin) Create
- `PUT /api/Categories/{id}` (Admin) Update
- `DELETE /api/Categories/{id}` (Admin) Delete

Products (`/api/Products`):

- `GET /api/Products` List products
- `GET /api/Products/category/{categoryId}` By category
- `GET /api/Products/{id}` Get product
- `POST /api/Products` (Admin) Create
- `PUT /api/Products/{id}` (Admin) Update
- `DELETE /api/Products/{id}` (Admin) Delete

Orders (`/api/Orders`):

- `GET /api/Orders` (Admin) List all orders
- `GET /api/Orders/{id}` Get order
- `GET /api/Orders/by-user` Current user orders
- `POST /api/Orders` (Self) Create order
- `PUT /api/Orders/{id}/status` Update status
- `DELETE /api/Orders/{id}` (Admin) Delete order

Order Items (mixed route base):

- `GET /api/order-items` (Admin) All items
- `GET /api/orders/{orderId}/items` Items by order
- `GET /api/products/{productId}/order-items` Items by product
- `GET /api/orders/{orderId}/items/{productId}` Single item by order + product
- `GET /api/order-items/{id}` Item by id
- `POST /api/orders/{orderId}/items` Add item(s) to order
- `PUT /api/order-items/{orderItemId}` Update item
- `DELETE /api/orders/{orderId}/items/{productId}` Remove item by composite
- `DELETE /api/order-items/{orderItemId}` (Admin) Remove item by id

## 14. Challenges & Solutions

| Challenge                                                | Explanation                                                                                           | Solution Implemented                                                                                                                                                |
| -------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Ensuring correct authorization for user-specific actions | Prevent users from acting on others' resources                                                        | Custom `Self` policy + handler compares JWT user id vs route id (improve logic to clean conditional).                                                               |
| Preventing stale product/category listings               | Mutations can make cached lists outdated                                                              | Centralized cache service (future: add invalidation on create/update/delete).                                                                                       |
| Tracking user activity for reengagement                  | Needed timestamp updates without blocking request pipeline                                            | Lightweight middleware updates `LastActiveUtc` asynchronously via `IUserActivityService`.                                                                           |
| Avoiding background job duplication                      | Job should run once daily                                                                             | Hangfire RecurringJob with stable id `reengage-inactive-7d`.                                                                                                        |
| Decoupling re-engagement workflow                        | Avoid synchronous email sending                                                                       | Publish message to RabbitMQ; consumer processes event.                                                                                                              |
| Containerizing infra vs API                              | API disabled in compose for iterative dev                                                             | Provided instructions to uncomment service; environment variables pattern documented.                                                                               |
| Handling order item uniqueness                           | Need consistent retrieval by order+product                                                            | Dedicated routes & composite lookup method in service.                                                                                                              |
| Connection string name mismatch                          | Code calls `GetConnectionString("Default")` while config key is `ConnectionStrings:DefaultConnection` | Align by renaming config key to `Default` or change code to use `GetConnectionString("DefaultConnection")`. README calls this out to avoid null connection strings. |

## 15. Troubleshooting & FAQ

- 403 Forbidden on protected endpoints: Ensure `Authorization: Bearer <token>` header, and token not expired.
- JWT validation failures: Confirm `Jwt:Key` is set (User Secrets or env var) and matches signing key used to generate tokens.
- Hangfire dashboard 404: Verify environment is Development or routing allows `/hangfire` path; confirm Postgres reachable.
- Redis connection errors in Docker: Use `Redis__Configuration=redis:6379` when running inside compose network.
- Migration errors: Check connection string host matches container (`bookstore_postgres` when API outside compose using host mapping or `postgres` when inside network depending on service name). Adjust `ConnectionStrings__DefaultConnection` accordingly.
- RabbitMQ auth failure: Ensure env vars `RABBITMQ_USER` & `RABBITMQ_PASS` exported before `docker compose up`.

## 16. Contributing & Next Steps

Potential enhancements:

- Add refresh tokens & token revocation list in Redis.
- Implement proper cache invalidation & more granular keys.
- Add pagination & filtering endpoints (products, orders) using `ProductQueryDto` & `PagedResultDto` fully.
- Introduce integration tests & CI pipeline for PRs.
- Expand messaging to include order lifecycle events.
- Add Health Checks endpoint `/health` for container orchestration.
- Harden `SelfAuthorizationHandler` logic & unit tests.

Contributions welcome: fork, create feature branch, open PR.

## 17. License

Currently no explicit license file present. Add a license (MIT/Apache-2.0) to clarify reuse.

---

### Swagger

Run locally then visit `/swagger`.

### Hangfire Dashboard

Accessible at `/hangfire` (secured via app-level auth if extended).

---

_Last updated: 2025-11-08_
