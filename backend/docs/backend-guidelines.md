# Backend Guidelines

## Overview

The backend is a `.NET 8` solution organized in a layered architecture:

- `esecai.API`: ASP.NET Core Web API entry point, HTTP controllers, middleware, and runtime bootstrapping.
- `esecai.Application`: DTOs, service/repository interfaces, and use-case classes.
- `esecai.Domain`: entities and domain exceptions.
- `esecai.Infrastructure`: EF Core persistence, repositories, external integrations, auth helpers, SignalR hub, and migrations.
- `esecai.Tests`: xUnit-based tests plus test artifacts for PDF/AI extraction flows.

The solution file is `esecai.sln`. The API project is the startup project used by local runs, EF migrations, and container publish.

## Runtime Setup

### Entry point

`esecai.API/Program.cs` configures:

- forwarded headers support for proxy/tunnel deployments
- infrastructure and application dependency injection
- controller JSON enum serialization via `JsonStringEnumConverter`
- Swagger/OpenAPI
- CORS for:
  - `https://esecai.paoloaraneta.dev`
  - `http://127.0.0.1:3000`
  - `http://localhost:3000`
- fixed-window rate limiting for auth and OTP endpoints
- SignalR notifications hub at `/hubs/notifications`
- automatic EF Core migration on startup with retry logic
- authentication, authorization, and controller endpoint mapping

### Main dependencies

The current backend setup depends on:

- PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL`
- Redis via `StackExchange.Redis` and `NRedisStack`
- MinIO for object/file storage
- JWT auth plus secure cookie transport
- Google OAuth for sign-in
- SignalR for notifications
- PDF parsing via `PdfPig`
- AI extraction/generation via Gemini and Ollama

### Local infrastructure

`docker-compose.yml` currently provisions:

- `esecai-db`: PostgreSQL on host port `5433`
- `esecai-cache`: Redis on host port `6379`
- `esecai-storage`: MinIO on host ports `9000` and `9001`
- `esecai-tunnel`: Cloudflare tunnel sidecar

The API container block exists but is commented out. Local API execution is currently driven more directly through `dotnet run` or IDE launch profiles.

### Launch profiles

`esecai.API/Properties/launchSettings.json` defines:

- `http`: binds to `http://0.0.0.0:8080`
- `https`: binds to `https://localhost:7179` and `http://localhost:5251`
- `IIS Express`

## Configuration

### Expected configuration sections

`appsettings.json` and environment variables feed:

- `ConnectionStrings:DefaultConnection`
- `ConnectionStrings:Redis`
- `Jwt`
- `Authentication:Google`
- `Email`
- `Minio`
- `AI`

### AI options

`AIOptions` is bound from `AI` config, then post-processed in `Infrastructure/DependencyInjection.cs` to also support env-var-style fallbacks such as:

- `AI_OllamaBaseUrl`
- `AI_OllamaApiEndpoint`
- `AI_OllamaApiKey`
- `AI_GeminiApiKey`
- `AI_GeminiModel`
- `Gemini__ApiKey`
- `Gemini__Model`

### Configuration rule

Keep secrets out of committed config files. Production values should come from environment variables or secret storage, not hardcoded JSON.

## Layer Responsibilities

### API layer

Controllers currently expose:

- `AuthController`: login, signup, refresh, email verification, Google OAuth, logout, profile
- `ClassroomController`: create, update, list, detail, delete
- `AssessmentController`: create and list assessments
- `QuestionController`: get assessment questions
- `RecordController`: scaffold only, not yet implemented

Controller responsibilities in this codebase:

- parse HTTP-specific inputs like `IFormFile`
- extract authenticated user claims
- translate request payloads into application DTOs
- return HTTP responses and status codes

Business logic should stay in use cases and infrastructure services, not accumulate in controllers.

### Application layer

`esecai.Application` contains:

- DTOs for auth, classroom, assessment, question, PDF, and AI options
- interfaces for repositories and services
- use cases grouped by feature: `Auth`, `Classroom`, `Assessment`, and `Question`

This layer is the orchestration boundary between HTTP input and infrastructure implementations.

### Domain layer

`esecai.Domain` currently holds:

- entities: `User`, `Classroom`, `Assessment`, `Question`, `Record`, `RecordAnswer`, `Notification`
- domain exceptions: `DomainException`, `EmailAlreadyExistException`, `EmailNotVerifiedException`

Domain factory methods such as `Classroom.Build(...)` and `Assessment.Build(...)` are already in use for controlled creation.

### Infrastructure layer

`esecai.Infrastructure` owns:

- `AppDbContext` and `UnitOfWork`
- EF Core migrations
- repository implementations
- auth token generation
- Redis cache access
- email dispatch
- MinIO file handling
- notification service and SignalR hub
- PDF extraction/debugging
- AI client integration with Gemini/Ollama fallback behavior

## Data Model

`AppDbContext` defines DbSets for:

- `Users`
- `Classrooms`
- `Assessments`
- `Questions`
- `Records`
- `RecordAnswers`
- `Notifications`

Current relationship direction:

- `User` -> many `Classroom`
- `Classroom` -> many `Assessment`
- `Assessment` -> many `Question`
- `Assessment` -> many `Record`
- `Record` -> many `RecordAnswer`
- `Question` -> many `RecordAnswer`

`SaveChangesAsync` also auto-populates `*_created_at` and `*_updated_at` timestamp fields when present.

## Auth and Security Setup

Authentication is configured in `Infrastructure/DependencyInjection.cs` with:

- default JWT bearer auth
- cookie sign-in support for the Google OAuth flow
- Google OAuth callback at `/api/auth/signin-google`

JWT retrieval behavior:

- first from `accessToken` cookie
- fallback to `access_token` query param for SignalR hub connections

Current platform protections wired in startup:

- CORS allowlist
- auth/OTP rate limiting
- forwarded headers for proxy deployment
- authz middleware

Recommended conventions for future backend work:

- validate input at controller/use-case boundaries
- keep secrets outside source control
- avoid returning stack traces in API responses outside local development
- keep external-service code behind application interfaces

## Storage and Messaging

### Database

- PostgreSQL is the system of record.
- EF Core migrations live in `esecai.Infrastructure/Migrations`.
- `Program.cs` applies pending migrations automatically at startup.

### Cache

- Redis is used through `IDatabase` from `StackExchange.Redis`.
- OTP and short-lived auth workflows already depend on Redis-backed flows.

### Object storage

- MinIO is configured through `AddMinio(...)`.
- Classroom and assessment file workflows should keep file handling behind `IMinioFileService`.

### Realtime

- SignalR is enabled via `AddSignalR()`.
- Notifications hub route: `/hubs/notifications`.

## AI and PDF Pipeline

The backend includes a document-processing flow for assessments:

1. `PdfService` detects embedded text.
2. Digital PDFs are parsed and normalized with `PdfPig`.
3. AI extraction is handled by `AIClientService`.
4. Primary provider is configurable as Ollama or Gemini.
5. Fallback between Ollama and Gemini is supported when enabled.

Prompt assets currently live in:

- `esecai.Infrastructure/Services/Docs/GeminiPrompts.cs`
- `esecai.Infrastructure/Services/Docs/OllamaPrompts.cs`
- `esecai.Infrastructure/Services/Docs/assessment-extraction.md`

## Developer Commands

The backend `Makefile` currently defines:

- `make d-build`: compose up with build
- `make d-up`: compose up detached
- `make d-down`: compose down
- `make mig-add name=YourMigrationName`: add EF migration
- `make db-update`: apply EF migrations
- `make d-db`: open PostgreSQL shell inside the DB container

## Testing

`esecai.Tests` uses:

- `xUnit`
- `Moq`
- `coverlet.collector`

Current tests focus on:

- `PdfService`
- `AIClientService`

The test project also stores generated extraction artifacts in `esecai.Tests/TestOutput/`.

## Structure Notes

- `Class1.cs` exists in `esecai.Application` and `esecai.Domain` as template leftovers.
- `RecordController` is present but not yet implemented.
- Backend docs live under `backend/docs/`.
- IDE/build output directories such as `bin/`, `obj/`, and `.vs/` exist locally but should not drive architectural documentation.
