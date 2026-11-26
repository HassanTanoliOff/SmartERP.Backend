# AGENTS.md — SmartERP.Backend

Project context and conventions for AI coding agents and models working in this repository.

## Project Overview

**SmartERP.Backend** is a **.NET 10 (`net10.0`)** ASP.NET Core Web API backend for a modular Enterprise Resource Planning (ERP) system backed by **PostgreSQL** and **Entity Framework Core**.

Core business areas modeled in the system:
- **Identity, RBAC & Auditing**: Users, roles, login lockout tracking, audit logs.
- **Inventory & Warehousing**: Categories, products, warehouses, stock levels, stock movements.
- **Procurement**: Suppliers, purchase orders, purchase order items, goods receipts.
- **Sales & Finance**: Customers, sales orders, sales order items, invoices, payments.
- **HR**: Departments, employees, attendance.

---

## Solution & Architecture

Clean/Layered Architecture defined in `SmartERP.Backend.slnx`:

```text
SmartERP.Backend/
├── SmartERP.Backend.slnx
├── AGENTS.md
├── docs/                           # Project specs (e.g., SRS PDF / architecture docs)
└── src/
    ├── Core/                       # Domain layer (Entities, Enums) — zero NuGet dependencies
    ├── Appliaction/                # Application layer (csproj: Application.csproj, namespace: Application)
    ├── Infra/                      # Infrastructure layer (AppDbContext, EF Configurations, Repositories)
    └── SmartERP.API/               # Presentation layer (Controllers, Program.cs, appsettings)
```

> **Important naming quirk**: The Application project directory is spelled `src/Appliaction/` on disk, while the project file is `Application.csproj` and the root C# namespace is `Application`. Do not rename this folder unless explicitly asked, as `.slnx` and `.csproj` references depend on it.

### Project Dependencies & Packages
- **`Core`** (`src/Core/Core.csproj`): Domain entities and enums only.
- **`Application`** (`src/Appliaction/Application.csproj`): References `Core`. Uses `AutoMapper` (`16.2.0`), `FluentValidation.DependencyInjectionExtensions` (`12.2.0`). Registers services via `AddApplication()` in `DependencyInjection.cs`.
- **`Infra`** (`src/Infra/Infra.csproj`): References `Application`. Uses `Microsoft.EntityFrameworkCore` (`10.0.14`), `Npgsql.EntityFrameworkCore.PostgreSQL` (`10.0.3`). Registers `AppDbContext` and repositories via `AddInfrastructure()` in `DependencyInjection.cs`.
- **`SmartERP.API`** (`src/SmartERP.API/SmartERP.API.csproj`): References `Application` and `Infra`. Uses `Microsoft.AspNetCore.Authentication.JwtBearer` (`10.0.14`), `Microsoft.AspNetCore.OpenApi` (`10.0.12`), `Microsoft.EntityFrameworkCore.Design` (`10.0.14`), `Swashbuckle.AspNetCore` (`10.3.1`).

All projects use `<TargetFramework>net10.0</TargetFramework>`, `<Nullable>enable</Nullable>`, and `<ImplicitUsings>enable</ImplicitUsings>`.

---

## Current Implementation Status

When starting a task, note what is already built versus what is still scaffolded:

- **Implemented**:
  - All 19 domain entities (`src/Core/Entities/`) and 9 enums (`src/Core/Enums/`).
  - `AppDbContext` (`src/Infra/DBContext/AppDbContext.cs`) with `DbSet<T>` for all 19 entities.
  - All 19 Fluent API entity configurations (`src/Infra/EntitiesConfigurations/`).
  - OpenAPI + Swagger UI setup in `src/SmartERP.API/Program.cs` (enabled in Development).
  - `BaseController` and stub controllers (`HealthCheckController`, `AuthController`, `UserController`, `CustomerController`).
- **Not Yet Implemented / Scaffolded**:
  - Application services, interfaces, DTOs, AutoMapper profiles, and FluentValidation validators (`AddValidatorsFromAssembly` is wired; `AddAutoMapper` is commented out in `src/Appliaction/DependencyInjection.cs`).
  - Repository interfaces and implementations.
  - JWT Bearer authentication registration/middleware (`JwtBearer` package is installed, `app.UseAuthorization()` is in `Program.cs`, but `AddAuthentication()` / `UseAuthentication()` are not configured yet).
  - EF Core migrations (no `Migrations/` folder yet).
  - Automated test projects.
  - Placeholder `Class1.cs` files still exist in `Core`, `Appliaction`, and `Infra`.

---

## Domain Model Reference

### Entities (`src/Core/Entities/`) & `AppDbContext`
- **Identity & Audit**: `User` (table: `UsersDetails`), `AuditLog` (`AuditLogs`)
- **Inventory**: `Category`, `Product`, `Warehouse`, `Stock`, `StockMovement`
- **Procurement**: `Supplier`, `PurchaseOrder`, `PurchaseOrderItem`, `GoodsReceipt`
- **Sales & Finance**: `Customer`, `SalesOrder`, `SalesOrderItem`, `Invoice`, `Payment`
- **HR**: `Department`, `Employee`, `Attendance`

### Enums (`src/Core/Enums/`)
- `UserRoles`: `Anonymous`, `Admin`, `InventoryManager`, `PurchaseOfficer`, `PurchaseManager`, `SalesOfficer`, `Accountant`, `HROfficer`, `SuperAdmin`
- `AttendanceStatus`: `Present`, `Absent`, `OnLeave`
- `InvoiceStatus`: `NotSet`, `Unpaid`, `PartiallyPaid`, `Paid`, `Cancelled`
- `PaymentMethods`: `Unset`, `Cash`, `Card`, `BankTransfer`, `Cheque`
- `PurchaseOrderStatus`: `NotSet`, `Draft`, `Approved`, `Received`, `Cancelled`
- `SalesOrderStatus`: `NotSet`, `Draft`, `Confirmed`, `Delivered`, `Cancelled`
- `StockMovementReferenceTypes`: `UnKnownType`, `PurchaseOrder`, `SalesOrder`, `Transfer`, `Adjustment`, `Return`, `Opening`
- `StockMovementType`: `Unknown`, `Receipt`, `Sale`, `Adjustment`, `TransferOut`, `TransferIn`
- `UnitOfMeasure`: `Unset`, `Packet`, `Pack`, `Box`, `Grams`, `Kilograms`, `Pounds`

---

## Coding & Database Conventions

Follow these existing patterns when adding features:

1. **Controllers**:
   - Inherit from `BaseController` (`src/SmartERP.API/Controllers/BaseController.cs`), which sets `[ApiController]`, `[Route("api/[controller]")]`, and `[Produces("application/json")]`.
   - Keep business logic out of controllers; delegate to Application-layer services.
2. **EF Core Configurations**:
   - Place one `IEntityTypeConfiguration<T>` class per entity in `src/Infra/EntitiesConfigurations/`. They are auto-registered in `AppDbContext.OnModelCreating` via `ApplyConfigurationsFromAssembly`.
   - Store enums as strings: `.HasConversion<string>().HasMaxLength(...)`.
   - Use PostgreSQL `now()` for UTC timestamps (`.HasDefaultValueSql("now()")`).
   - Use `DateOnly` mapped to `.HasColumnType("date")` for plain calendar dates (e.g., `OrderDate`).
   - Use `.HasPrecision(18, 2)` for currency/decimal amounts.
   - Prefer `DeleteBehavior.Restrict` on foreign keys unless cascading delete is explicitly intended.
3. **Dependency Injection**:
   - Register Application services/validators/mappings in `src/Appliaction/DependencyInjection.cs` (`AddApplication`).
   - Register DbContext and repositories in `src/Infra/DependencyInjection.cs` (`AddInfrastructure`).

---

## Common Commands

Run commands from the repository root (`SmartERP.Backend/`):

```powershell
# Restore & build the solution
dotnet build SmartERP.Backend.slnx

# Run the API (Development: http://localhost:5062, https://localhost:7291)
dotnet run --project src/SmartERP.API/SmartERP.API.csproj

# Create an EF Core migration (once ready)
dotnet ef migrations add <MigrationName> --project src/Infra/Infra.csproj --startup-project src/SmartERP.API/SmartERP.API.csproj

# Apply EF Core migrations
dotnet ef database update --project src/Infra/Infra.csproj --startup-project src/SmartERP.API/SmartERP.API.csproj
```

- **Swagger UI** (in Development): `http://localhost:5062/swagger` (or `https://localhost:7291/swagger`).
- **Database connection string**: Placeholders live under `ConnectionStrings:DefaultConnection` in `src/SmartERP.API/appsettings.json` (PostgreSQL database `SmartERP`). Keep real credentials/JWT secrets in `src/SmartERP.API/appsettings.Local.json` (loaded in `Program.cs`, ignored by `.gitignore` and `.aiignore`), User Secrets, or environment variables.

---

## Reference Documentation & Key SRS Rules

- **Session memory (`docs/ai/`)**:
  - **At the start of a session**: Read [`docs/ai/PROJECT_CONTEXT.md`](docs/ai/PROJECT_CONTEXT.md) first (and [`docs/ai/decisions.md`](docs/ai/decisions.md) / [`docs/SRS.md`](docs/SRS.md) as needed). For IDE/CLI agent setup questions, check [`docs/ai/tooling.md`](docs/ai/tooling.md).
  - **At the end of a session**: Write/update a short summary in `docs/ai/sessions/<YYYY-MM-DD>.md` and update `docs/ai/PROJECT_CONTEXT.md` (do not build an MCP for memory).
- **Full requirements specification**: [`docs/SRS.md`](docs/SRS.md) (Markdown conversion of `docs/Software Requirements Specification — SmartERP.pdf`, updated to target **PostgreSQL**). Read `docs/SRS.md` before implementing business logic, endpoints, or security rules.
- **Key cross-cutting SRS rules to follow in new code**:
  - **API versioning & errors**: Version endpoints (e.g., `/api/v1/...`), validate inputs with FluentValidation, and return RFC 7807 `ProblemDetails` (`400`, `401`, `403`, `404`, `409`, `422`, `500`).
  - **Pagination & DTOs**: Never expose EF entities from controllers; list endpoints must support pagination, sorting, and filtering with max page size `100`.
  - **Transactions & concurrency**: Wrap multi-step operations (goods receipt, SO delivery, stock transfer, payment recording) in a single DB transaction; handle `Stock.RowVersion` conflicts with `409 Conflict`.
  - **Security & RBAC**: JWT authentication (default 60-min expiry), 5-failed-login / 15-min lockout, server-side role checks (`401`/`403`), and segregation of duties (PO creator cannot approve their own PO).
  - **Audit logging**: Record append-only `AuditLog` entries (`old_values`, `new_values`, acting user, UTC timestamp) for creates, updates, deletes, approvals, and status changes; never log passwords or tokens.

---

## AI Working Style & Developer Preferences

- **Keep answers short and precise**.
- **Learning-focused & junior-friendly code**: Write straightforward, readable C# code rather than over-clever abstractions. Briefly explain *why* an approach was chosen and *what would be bad practice*.
- **Setup/troubleshooting steps**: Go one step at a time and wait for user confirmation before moving to the next step.
- **Secrets & privacy**: Never put secrets (connection strings, passwords, API keys, JWT secrets) in notes, markdown files, or prompts—free models may log prompts.

---

## User Trigger Keywords

When the user sends any of these keywords (or close variants), perform the corresponding action immediately:

| Keywords | Action |
| :--- | :--- |
| `start session`, `sync` | Read `docs/ai/PROJECT_CONTEXT.md` and check `git status`, then give a short recap of current state and next steps. |
| `end session`, `handoff`, `wrap up` | Write/update `docs/ai/sessions/<YYYY-MM-DD>.md`, update `docs/ai/PROJECT_CONTEXT.md` (and `docs/ai/decisions.md` if decisions were made), and list exact next steps (never log secrets). |
| `condense notes`, `compact memory` | Merge older `docs/ai/sessions/*.md` notes into `docs/ai/PROJECT_CONTEXT.md` to keep memory concise. |
| `log decision`, `record decision` | Append a short *what + why* entry to `docs/ai/decisions.md`. |
| `step by step`, `guide me` | Give **one** setup or troubleshooting step at a time and wait for user confirmation before continuing. |
| `explain this`, `why this` | Briefly explain why the approach was chosen in plain language and what would be bad practice. |
| `tooling help`, `check setup` | Read `docs/ai/tooling.md` and continue the next open Rider/CLI agent setup item one step at a time. |
