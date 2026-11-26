# Project Context — SmartERP.Backend

> **Living summary** for AI coding agents and the developer. Read this at the start of a session and update it at the end of a session.

## 1. Snapshot
- **Project**: `SmartERP.Backend` — backend REST API for an ERP system (Inventory, Procurement, Sales, Invoicing/Payments, HR, Reporting, Audit).
- **Tech Stack**: .NET 10 (`net10.0`), ASP.NET Core Web API, Entity Framework Core (`10.0.14`), PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL` `10.0.3`), AutoMapper (`16.2.0`), FluentValidation (`12.2.0`), Swashbuckle/OpenAPI.
- **IDE / OS**: JetBrains Rider on Windows.
- **Specs**: Full requirements in [`docs/SRS.md`](../SRS.md) (converted from the SRS PDF and adapted for PostgreSQL).

---

## 2. Current Implementation State

### Already Built
- **Core (`src/Core/`)**: All 19 domain entities (`Entities/`) and 9 enums (`Enums/`).
- **Infra (`src/Infra/`)**: `AppDbContext` with `DbSet<T>` for all 19 entities; 19 Fluent API `IEntityTypeConfiguration<T>` classes; `AddInfrastructure()` DI registration.
- **Application (`src/Appliaction/`)**: `AddApplication()` DI with `AddValidatorsFromAssembly` wired (`AddAutoMapper` commented out until profiles exist).
- **API (`src/SmartERP.API/`)**: `Program.cs` with controllers, OpenAPI, Swagger UI (Development), loads `appsettings.Local.json` (gitignored); `BaseController`; stub controllers (`HealthCheckController`, `AuthController`, `UserController`, `CustomerController`).
- **AI Docs**: `AGENTS.md` (with trigger keywords), `docs/SRS.md`, `docs/ai/` memory files, `.aiignore`.
- **Build**: Passes `0 errors` as of 2026-11-26.

### Pending Commit
All changes from the 2026-11-26 session are **not yet committed**. First action next session:
```powershell
git add -A
git commit -m "chore: set up AI docs, secrets hygiene, and gitignore cleanup"
git push
```

### Not Built Yet (Priority Order)
1. **EF Core migration** — create `InitialCreate` migration and apply to PostgreSQL (`dotnet ef migrations add` / `database update`).
2. **Error handling & API foundations** — global `ProblemDetails` middleware, API versioning (`/api/v1/...`), pagination helpers.
3. **Auth & RBAC** — JWT Bearer (`AddAuthentication`/`UseAuthentication`), login endpoint, password hashing, 5-failed-login/15-min lockout, role-based authorization.
4. **Audit logging** — append-only `AuditLog` entries (old/new values, acting user, UTC; never log passwords/tokens).
5. **Module implementation** — Repositories + Services/DTOs/Validators/AutoMapper + Controllers for: Users & Auth, Inventory, Procurement, Sales & Finance, HR, Reports.
6. **Automated tests** — unit tests (≥70% service coverage) and key integration tests.

---

## 3. Key Conventions & Gotchas
- **Folder spelling quirk**: Application layer folder on disk is `src/Appliaction/` (keep as-is; project file is `Application.csproj`, C# namespace is `Application`).
- **Code style**: Simple, clear, junior-friendly — no over-clever abstractions. Briefly explain *why* an approach is chosen and *what is bad practice*.
- **No secrets in files/prompts**: Real DB connection string lives in gitignored `src/SmartERP.API/appsettings.Local.json`. Never paste credentials into markdown notes or AI prompts.
- **EF / PostgreSQL conventions**:
  - Enums stored as strings (`HasConversion<string>()`).
  - UTC timestamps default to `now()`; date-only fields use `DateOnly` mapped to `date`.
  - Money uses `decimal(18,2)` (`HasPrecision(18, 2)`).
  - `Stock.RowVersion` (`uint`, `.IsRowVersion()`) handles optimistic concurrency → return `409 Conflict`.
  - Multi-step operations (goods receipt, SO delivery, stock transfer, payment) must run in a single DB transaction.

---

## 4. Open Tooling / Environment Items
See [`docs/ai/tooling.md`](./tooling.md) for Rider/CLI agent setup notes and remaining items (ACP Google auth switch, OpenCode PATH fix, Codex CLI install).
