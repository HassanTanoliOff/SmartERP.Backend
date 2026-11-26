# Architectural & Project Decisions

Record key technical and workflow decisions here so future sessions understand *why* things are set up the way they are.

---

## 1. Database: PostgreSQL instead of SQL Server
- **Decision**: Use PostgreSQL with `Npgsql.EntityFrameworkCore.PostgreSQL` instead of SQL Server (which was mentioned in the original v2.0 SRS PDF).
- **Why**: The codebase (`Infra.csproj`, `AppDbContext`, entity configurations) was already built for PostgreSQL (`now()`, `xmin`/`IsRowVersion()`, `date`). `docs/SRS.md` has been updated to match PostgreSQL.

## 2. Clean / Layered Architecture
- **Decision**: 4 projects — `Core` (entities/enums, no NuGet deps) → `Application` (`src/Appliaction/`, services/DTOs/validators/AutoMapper) → `Infra` (`AppDbContext`, EF configurations, repositories) → `SmartERP.API` (controllers, middleware, DI wiring).
- **Why**: Keeps controllers thin (no business logic) and domain rules independent of EF Core and HTTP concerns.
- **Note**: Keep the existing `src/Appliaction/` folder spelling unless explicitly refactoring solution paths.

## 3. EF Core Mapping Conventions
- **Decision**: One `IEntityTypeConfiguration<T>` per entity in `src/Infra/EntitiesConfigurations/`, enums stored as strings, `DateOnly` mapped to `date`, UTC timestamps defaulted with `now()`, `DeleteBehavior.Restrict` on FKs, `decimal(18,2)` for money, and `Stock.RowVersion` (`uint` / `IsRowVersion()`) for optimistic concurrency.
- **Why**: Readable enum values in SQL, clear separation of plain dates vs. UTC timestamps, protection against accidental cascade deletes, and safe concurrent stock updates (`409 Conflict`).

## 4. AI Memory via Markdown Files (No Memory MCP)
- **Decision**: Store context in `docs/ai/` (`PROJECT_CONTEXT.md`, `decisions.md`, `tooling.md`, `sessions/<date>.md`) instead of building a custom memory MCP server.
- **Why**: Works across all AI agents (`agy`, OpenCode, Kilo, Codex, Rider ACP) with zero infrastructure overhead, stays version-controlled, and is easy to read/condense (or open in Obsidian).
