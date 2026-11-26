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
- **Infra (`src/Infra/`)**: `AppDbContext` (`DBContext/AppDbContext.cs`) with `DbSet<T>` for all 19 entities and 19 `IEntityTypeConfiguration<T>` classes (`EntitiesConfigurations/`), plus `AddInfrastructure()` in `DependencyInjection.cs`.
- **Application (`src/Appliaction/`)**: `DependencyInjection.cs` (`AddApplication()`) with `AddValidatorsFromAssembly` wired (`AddAutoMapper` commented out until profiles exist).
- **API (`src/SmartERP.API/`)**: `Program.cs` with Controllers, OpenAPI, Swagger UI (Development), `AddApplication()`, `AddInfrastructure()`, plus `BaseController` and stub controllers (`HealthCheckController`, `AuthController`, `UserController`, `CustomerController`).
- **AI Docs**: `AGENTS.md`, `docs/SRS.md`, and `docs/ai/` memory files.

### Not Built Yet (Next Work)
1. **Database & Migrations**: Create initial EF Core migration, apply to PostgreSQL, and add seed data (Admin user, sample products/warehouses per NFR-12).
2. **Error Handling & API Foundations**: Global exception handling returning RFC 7807 `ProblemDetails` (`400`, `401`, `403`, `404`, `409`, `422`, `500`), API versioning (`/api/v1/...`), pagination/filtering/sorting helpers (max page size 100).
3. **Auth & RBAC**: JWT Bearer authentication (`AddAuthentication` / `UseAuthentication`), password hashing, login endpoint, 5-failed-login / 15-min lockout, role-based authorization.
4. **Audit Logging**: Append-only `AuditLog` recording (`old_values`, `new_values`, acting user, UTC timestamp; never log passwords/tokens).
5. **Module Implementation** (Repositories + Application Services/DTOs/Validators + Controllers):
   - Users & Auth
   - Inventory (Categories, Products, Warehouses, Stock, StockMovements, Transfers, Adjustments, Low-stock)
   - Procurement (Suppliers, PurchaseOrders, Approval with segregation of duties, GoodsReceipts)
   - Sales & Finance (Customers, SalesOrders, Confirmation/Delivery stock checks, Invoices, Payments)
   - HR (Departments, Employees, Attendance, salary visibility rules)
   - Reports (Sales, Inventory valuation, Purchases, Accounts receivable)
6. **Automated Tests**: Unit tests (target >= 70% service coverage) and key integration tests (NFR-11).

---

## 3. Key Conventions & Gotchas
- **Folder spelling quirk**: Application layer folder on disk is `src/Appliaction/` (keep as-is; project is `Application.csproj`, namespace is `Application`).
- **Code style**: Keep code simple, clear, and junior-friendly—avoid over-clever abstractions. Briefly explain *why* an approach is chosen and *what is bad practice*.
- **No secrets in files/prompts**: Never paste connection strings, passwords, JWT secrets, or API keys into markdown notes or AI prompts.
- **EF / PostgreSQL conventions**:
  - Enums stored as strings (`HasConversion<string>()`).
  - UTC timestamps default to `now()`; date-only fields use `DateOnly` (`date`).
  - Money uses `decimal(18,2)`.
  - `Stock.RowVersion` (`uint`, `.IsRowVersion()`) handles optimistic concurrency (`409 Conflict`).
  - Multi-step operations (goods receipt, SO delivery, stock transfer, payment) must run inside a single DB transaction.

---

## 4. Open Tooling / Environment Items
See [`docs/ai/tooling.md`](./tooling.md) for Rider/CLI agent setup notes and remaining tooling items.
