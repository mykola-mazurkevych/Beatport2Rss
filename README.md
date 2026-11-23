# Beatport2Rss

Beatport2Rss is a .NET project for collecting Beatport releases for followed artists and labels. Its goal is to make these releases available as RSS feeds. The project is under active development: account, feed, and subscription management, scheduled collection, and release processing are implemented, while RSS document generation is still in development.

## Current status

- **API:** versioned REST API for users and sessions, feeds, subscriptions, and tags.
- **Collector:** scheduled worker that retrieves releases from Beatport and publishes collected release events.
- **Builder:** processes release and management events and stores the corresponding data; it also removes stale releases.
- **Token interceptor:** gRPC service used by workers to obtain Beatport access tokens.
- **RSS output:** the API exposes stored RSS documents at `/feeds/{slug}/rss`; scheduled RSS document generation is still in development.

## Architecture

```mermaid
flowchart LR
    Clients --> API
    API --> PostgreSQL
    API --> RabbitMQ
    RabbitMQ --> Collector
    Collector --> Beatport
    Collector -- gRPC token --> Interceptor
    Collector --> PostgreSQL
    Collector --> RabbitMQ
    RabbitMQ --> Builder
    Builder --> PostgreSQL
    API -- gRPC RSS fetch --> Builder
```

The API and workers use PostgreSQL for persistence and RabbitMQ for asynchronous integration events. The collector calls Beatport and obtains access tokens through the interceptor service. The builder processes collected releases and API events.

## Technology

- .NET 10 / C# 14
- ASP.NET Core minimal APIs
- PostgreSQL and Entity Framework Core
- RabbitMQ and Wolverine
- Quartz scheduled jobs
- Docker Compose

## Run with Docker Compose

Prerequisites: Docker with the Compose plugin and valid Beatport credentials.

From the repository root, create and edit the local environment file:

```powershell
Set-Location docker
Copy-Item .env.example .env
```

Update `docker/.env` with your Beatport credentials and secure values for the database, RabbitMQ, JWT, and Resend settings. Do not commit this file; local `.env` files are ignored by Git.

Start the services in the same terminal:

```powershell
docker compose up --build
```

Compose starts PostgreSQL, RabbitMQ, the token interceptor, separate API, Builder, and Collector migrators, the API and workers, and the standalone web frontend. Each migrator runs after PostgreSQL is healthy and applies migrations only to its service's database. The API and workers wait for their own migrator to finish successfully, so a migration failure in one service does not block the others.

By default, the API is available at `http://localhost:8080` and its development API reference at `http://localhost:8080/scalar`. RabbitMQ management is available at `http://localhost:15672`. Ports and service settings can be changed in `docker/.env`.

Stop the services with:

```powershell
docker compose down
```

## Frontend

The Blazor frontend is located in `web/src/Beatport2Rss.Web` and is also included in the normal full-stack Compose startup. To run only the web service, use the commands below from the `docker` directory:

```powershell
docker compose up -d --build web
docker compose ps web
docker compose logs web
```

Browse to `http://localhost:8081` by default. Set `WEB_PORT` in `docker/.env` to use a different host port; the container continues to listen on port 8080. If you need a local Compose environment file, create one from the template with `Copy-Item .env.example .env`.

Starting only `web` does not start or require the API, database, messaging services, or valid Beatport credentials. Rebuild the frontend after making changes with:

```powershell
docker compose up -d --build web
```

Stop only the frontend with:

```powershell
docker compose stop web
```

For local development without Docker, run the web project from the repository root:

```powershell
dotnet run --project .\web\src\Beatport2Rss.Web\Beatport2Rss.Web.csproj
```

The frontend starts independently of the API and other backend services. The confirmation page uses the server-side `ApiBaseUrl` (default `http://localhost:5162`); with Compose the web service listens on `http://localhost:8081` and calls the API at `http://api:8080`. It shows a retryable error if the API is unavailable.

## Email confirmation

Registration always creates a Pending account. To enable automatic delivery, set `EmailConfirmation__Enabled=true` on the API (or `EMAIL_CONFIRMATION_ENABLED=true` in Compose) and configure:

| API setting | Compose variable | Purpose |
| --- | --- | --- |
| `ResendOptions__ApiToken` | `RESEND_API_TOKEN` | Valid Resend API key. Keep it in API user secrets, environment or a secret manager, never in the web container. |
| `EmailConfirmation__TemplateId` | `EMAIL_CONFIRMATION_TEMPLATE_ID` | ID or alias of a published Resend confirmation template. |
| `EmailConfirmation__PublicWebBaseUrl` | `EMAIL_CONFIRMATION_PUBLIC_WEB_BASE_URL` | Public, browser-reachable web URL used in links, not the API address. Use HTTPS in production. |
| `EmailConfirmation__Lifetime` | `EMAIL_CONFIRMATION_LIFETIME` | Positive duration, default `1.00:00:00` (24 hours), maximum 365 days. |

Configure the published template with a verified sender and subject, and use the variables `CONFIRMATION_URL`, `EXPIRES_AT` (UTC), and `VALID_HOURS` (hours from issuance). It should tell recipients to confirm their address and that they can ignore the message if they did not sign up. Resend supplies the rendered HTML and plain text. When upgrading from the inline sender, replace `RESEND_API_KEY` with `RESEND_API_TOKEN` and move the sender settings into the template.

When delivery is disabled (the default), no new confirmation or delivery work is generated, and public confirmation links cannot be used. When enabling delivery, explicitly set `EmailConfirmation__PublicWebBaseUrl` (Compose: `EMAIL_CONFIRMATION_PUBLIC_WEB_BASE_URL`) to the browser-reachable URL of the web application; there is no localhost fallback. To allow manual activation, set `EmailConfirmation__AdministratorUserId` (Compose: `EMAIL_CONFIRMATION_ADMIN_USER_ID`) to the immutable database UUID of a trusted, operator-verified existing account. Obtain this ID through trusted database access, not an email address supplied by a registrant. That authenticated account can use `PUT /users/{id}/activation` with its bearer token to activate **another** user regardless of whether email delivery is enabled. Activation changes account status only; it does not create or confirm an email confirmation and is not proof of email ownership. Without a configured administrator, nobody can use this operation.

Opening a confirmation email link only reads state; the recipient must press **Confirm email**. The API does not sign the recipient in. Expiry does not change account status; a completed confirmation can still be acknowledged, but cannot reactivate a user deactivated after confirmation. A durable local `UserCreated` handler stages the confirmation and private delivery message together; the lifetime still starts at registration. Replayed events can create multiple confirmations and send multiple emails. The Wolverine delivery job checks that its confirmation is still applicable before sending, retries transient failures and uses a stable Resend idempotency key. This does not guarantee exactly-once delivery. Confirmation IDs are secret link credentials: restrict access to the confirmation table, Wolverine message tables, backups, dead letters, and provider idempotency keys. Configure external access logs and telemetry to omit/redact the full `/api/email-confirmations/{id}` path and `/confirm-email` query string. Do not enable provider link tracking for these capability URLs.

**Database handoff:** Before deployment, manually add the EF migration for `api."EmailConfirmations"`: UUID primary key `Id`; non-null UUID `UserId` (indexed FK to `api."Users"."Id"` with cascade delete); required `EmailAddress` using the existing user email column length; required `CreatedAt` and `ExpiresAt` and nullable `ConfirmedAt` (all `timestamp with time zone`). Existing hash-based links cannot be converted to the new ID-based links and must be reissued. No migration is included here; the Compose API migrator requires the new migration even when delivery is disabled. The delivery queue uses the existing `api_messaging` Wolverine storage.

The landing page and the `/register` and `/login` informational placeholders run independently of the API and other backend services. Use the shared language switch to choose English or Ukrainian; the choice is remembered in the browser.

## Build and tests

Install the .NET 10 SDK, then run these commands from the repository root:

```powershell
dotnet restore Beatport2Rss.slnx
dotnet test Beatport2Rss.slnx
```

The confirmation concurrency integration test also runs against PostgreSQL when `EMAIL_CONFIRMATION_TEST_POSTGRES` is set to a local test-server connection string with permission to create and drop a disposable database. No real emails or Resend credentials are needed for tests.

## Repository layout

| Path | Contents |
| --- | --- |
| `api/` | REST API, application/domain logic, persistence, and tests |
| `web/` | Standalone Blazor frontend |
| `services/collector/` | Beatport release collection worker |
| `services/builder/` | Release and feed-related event processing worker |
| `services/interceptor/` | Beatport token interception gRPC service |
| `common/` | Shared domain, Beatport client, persistence, and messaging components |
| `tools/` | Database migrator and source generator |
| `docker/` | Docker Compose configuration and environment template |
