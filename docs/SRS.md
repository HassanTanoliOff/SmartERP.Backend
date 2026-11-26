# Software Requirements Specification — SmartERP

**Version:** 2.0 (revised, adapted to PostgreSQL) | **Author:** [Your Name / Roll No.] | **Date:** November 2026 | **Status:** Draft for review

> **Source note:** Converted from [Software Requirements Specification — SmartERP.pdf](./Software%20Requirements%20Specification%20%E2%80%94%20SmartERP.pdf) with database references updated from SQL Server to **PostgreSQL** to match the codebase (`Npgsql.EntityFrameworkCore.PostgreSQL`).

## Revision History

| Version | Change |
| :--- | :--- |
| 1.0 | Initial specification |
| 2.0 | Fixed gaps and contradictions, added missing entities, status rules, validation rules, security, and acceptance criteria |
| 2.0-pg | Converted to Markdown and updated database target from SQL Server to PostgreSQL |

---

## 1. Introduction

### 1.1 Purpose
This document defines the functional and non-functional requirements of SmartERP, a backend-focused Enterprise Resource Planning (ERP) system for small and medium-sized businesses. It is written for the developer and any reviewer as part of a personal project to practice backend development with the .NET platform.

### 1.2 Scope
SmartERP is a web-based ERP system that lets a business manage inventory, purchasing, sales, invoicing, employees, and reports from one system. It exposes secure REST APIs, tested through Swagger UI or Postman. A frontend is out of scope for this phase.

- **Technology:** ASP.NET Core Web API (C#), PostgreSQL, Entity Framework Core, JWT authentication.
- **Out of scope (this phase):** frontend, payroll, tax calculation, payment gateways, multi-company, multi-currency, batch/lot/serial tracking, partial deliveries and receipts, returns and credit notes.

### 1.3 Definitions

| Term | Definition |
| :--- | :--- |
| **ERP** | Enterprise Resource Planning; software that integrates core business processes such as inventory, sales, purchasing, and HR. |
| **SKU** | Stock Keeping Unit; a unique code identifying a product. |
| **Purchase Order (PO)** | A document sent to a supplier requesting goods at an agreed price and quantity. |
| **Goods Receipt** | A record confirming that ordered goods were received into a warehouse. |
| **Sales Order (SO)** | A document recording a customer's request to buy products. |
| **Stock Movement** | A record of any change in product quantity (receipt, sale, adjustment, transfer). |
| **RBAC** | Role-Based Access Control; restricting access based on a user's role. |
| **JWT** | JSON Web Token; a signed token used to authenticate each API request. |
| **Soft delete** | Marking a record inactive instead of removing it from the database. |
| **Optimistic concurrency** | Detecting conflicting updates with a version column instead of locking rows. |

---

## 2. Overall Description

### 2.1 Product Perspective
SmartERP is a standalone system made of an ASP.NET Core Web API (business logic), Entity Framework Core (data access), and a **PostgreSQL** database. Code is organized into controllers, services, repositories, and data models. Modules (Inventory, Procurement, Sales, Finance, HR) are integrated: a goods receipt increases stock and a delivered sales order decreases it.

### 2.2 User Roles
Each user has **exactly one role** in this version.

| Role | Responsibilities |
| :--- | :--- |
| **Admin** | Manages users, roles, settings, audit log; access to all modules; can approve purchase orders. |
| **Inventory Manager** | Manages categories, products, warehouses, stock, adjustments, transfers. |
| **Purchase Officer** | Manages suppliers; creates and edits purchase orders; records goods receipts. |
| **Purchase Manager** | Approves or rejects purchase orders (new role; replaces the vague "approval rights" in v1.0). |
| **Sales Officer** | Manages customers and sales orders; generates invoices. |
| **Accountant** | Records payments; views financial reports. |
| **HR Officer** | Manages departments, employees, and attendance. |

> **Segregation of duties:** A user cannot approve a purchase order they created.
> *(Codebase note: `Core.Enums.UserRoles` also defines `Anonymous = 0` as a default unset value and `SuperAdmin = 8`.)*

### 2.3 Assumptions and Constraints
1. Single company (single tenant).
2. Single currency; all amounts use the same currency.
3. Stock is tracked per product per warehouse.
4. Orders are processed in full; no partial deliveries or receipts.
5. Business rules (for example no negative stock) are enforced in application logic and by database constraints.
6. Payroll, tax, and payment gateways are out of scope.
7. API only; no graphical interface.
8. All timestamps are stored in UTC. Dates without time (order date, due date) are plain dates.
9. Stock is reduced only on delivery. Confirmation checks availability but does not reserve stock, so availability is checked again at delivery.

---

## 3. Functional Requirements

### 3.1 User and Access Management
- **FR-1:** An Admin shall create user accounts with name, email, password, and role.
- **FR-2:** Users shall log in with email and password and receive a JWT on success. Inactive users shall not be able to log in.
- **FR-3:** Email addresses shall be unique (case-insensitive).
- **FR-4:** Passwords shall be stored as salted hashes (for example ASP.NET Core Identity `PasswordHasher` or BCrypt), never in plain text.
- **FR-5:** Each endpoint shall be restricted by the authenticated user's role.
- **FR-6:** An Admin shall activate, deactivate, or change the role of a user. An Admin shall not be able to deactivate or demote themselves if they are the last active Admin.
- **FR-6a:** Passwords shall have at least 8 characters with a mix of letters and digits. After 5 failed logins in a row, the account shall be locked for 15 minutes.
- **FR-6b:** Users shall be able to change their own password by providing the current one.

### 3.2 Product and Inventory Management
- **FR-7:** Authorized users shall create, edit, and view categories. Names are unique.
- **FR-8:** Authorized users shall create a product with name, SKU, category, unit price, cost price, unit of measure, reorder level, and optional description. Prices shall be zero or greater; reorder level shall be zero or greater.
- **FR-9:** SKUs shall be unique and shall not change after the product is used in any order.
- **FR-10:** Authorized users shall edit and deactivate products. A product referenced by any order cannot be hard-deleted, only deactivated. Inactive products cannot be added to new orders.
- **FR-11:** Authorized users shall create and manage warehouses (name and location). A warehouse with stock cannot be deleted; it can be deactivated once its stock is zero.
- **FR-12:** The system shall maintain the current quantity of each product in each warehouse.
- **FR-13:** Every stock change shall create a stock movement with product, warehouse, signed quantity (positive in, negative out), movement type, reference type and ID, user, and date.
- **FR-14:** An Inventory Manager shall make manual stock adjustments with a mandatory reason.
- **FR-15:** No operation shall reduce stock of a product in a warehouse below zero.
- **FR-16:** Authorized users shall transfer stock between two different warehouses. A transfer creates two linked movements (out of source, into destination) in one transaction.
- **FR-17:** The system shall list products whose total stock across all warehouses is at or below the reorder level as low-stock items.
- **FR-18:** Users shall search and filter products by name, SKU, category, and status, with pagination.

### 3.3 Procurement Management
- **FR-19:** A Purchase Officer shall create, edit, and view suppliers (name, contact details, address). Supplier email shall be unique if provided.
- **FR-20:** A Purchase Officer shall create a purchase order with supplier, order date, and one or more line items (product, quantity greater than 0, unit cost zero or greater). A product shall appear only once per order.
- **FR-21:** The system shall calculate the PO total from its line items (sum of `quantity × unit cost`).
- **FR-22:** PO statuses and allowed transitions:
  - `Draft` → `Approved`, `Draft` → `Cancelled`
  - `Approved` → `Received`, `Approved` → `Cancelled`
  - `Received` and `Cancelled` are final.
- **FR-23:** Only an Admin or Purchase Manager shall approve a PO, and never the user who created it.
- **FR-24:** A PO can be edited only in `Draft` status.
- **FR-25:** A goods receipt shall be recorded against an approved PO with a chosen warehouse. It increases stock of every line item in that warehouse, creates stock movements, and changes the PO status to `Received`, all in one transaction.

### 3.4 Sales Management
- **FR-26:** A Sales Officer shall create, edit, and view customers (name, email, phone, address).
- **FR-27:** A Sales Officer shall create a sales order with customer, order date, source warehouse, and one or more line items (product, quantity greater than 0). The unit price shall default to the product's current price and is copied into the line item (price snapshot), so later price changes do not affect existing orders.
- **FR-28:** The system shall calculate subtotal (sum of `quantity × unit price`), discount, and total. The discount is a fixed amount or a percentage at order level and cannot exceed the subtotal.
- **FR-29:** SO statuses and allowed transitions:
  - `Draft` → `Confirmed`, `Draft` → `Cancelled`
  - `Confirmed` → `Delivered`, `Confirmed` → `Cancelled`
  - `Delivered` and `Cancelled` are final.
- **FR-30:** On confirmation the system shall check that stock in the source warehouse covers all line items and reject the confirmation with a clear message otherwise. After confirmation, the order is read-only.
- **FR-31:** On delivery the system shall check stock again, reduce stock, and record stock movements in one transaction. Delivery is rejected if stock is no longer sufficient.
- **FR-32:** `Draft` and `Confirmed` orders can be cancelled; `Delivered` orders cannot. A `Confirmed` order that has an invoice with payments cannot be cancelled. If it has an unpaid invoice, that invoice shall be set to `Cancelled`.

### 3.5 Invoicing and Payments
- **FR-33:** The system shall generate an invoice with a unique, sequential invoice number (for example `INV-2026-000001`) for a confirmed sales order. The invoice total equals the sales order total.
- **FR-34:** Only one invoice may exist per sales order.
- **FR-35:** An Accountant shall record one or more payments against an invoice with amount (greater than 0), date, and payment method (`Cash`, `Bank Transfer`, `Card`, `Cheque`). Payments cannot be recorded against a `Cancelled` invoice.
- **FR-36:** Total payments shall never exceed the invoice total.
- **FR-37:** Invoice status shall update automatically: `Unpaid` (no payments), `Partially Paid`, `Paid` (total reached), or `Cancelled`.
- **FR-38:** The due date is the issue date plus payment terms (default 30 days, configurable). An invoice is overdue when its due date has passed and its status is `Unpaid` or `Partially Paid`. Overdue is calculated when queried, not stored.
- **FR-38a:** Payments shall not be edited or deleted. Mistakes are corrected by an Admin through a documented reversal entry (recorded in the audit log).

### 3.6 Human Resource Management
- **FR-39:** An HR Officer shall create and manage departments with unique names. A department with active employees cannot be deleted.
- **FR-40:** An HR Officer shall create employee records (name, email, phone, department, designation, hire date, salary). Hire date cannot be in the future; salary must be greater than 0.
- **FR-41:** An HR Officer shall deactivate employees instead of deleting them.
- **FR-42:** An HR Officer shall record daily attendance as `Present`, `Absent`, or `On Leave`. Attendance cannot be recorded for inactive employees or for dates before their hire date.
- **FR-43:** Only one attendance record is allowed per employee per date. Records can be corrected by an HR Officer (changes are audited).
- **FR-44:** Authorized users shall view an employee's monthly attendance summary (count of each status).
- **FR-44a:** Salary data shall be visible only to the Admin and HR Officer roles.

### 3.7 Reporting and Audit
- **FR-45:** Sales report: total sales, number of orders, and top-selling products for a date range, counting `Delivered` orders only.
- **FR-46:** Inventory valuation report: quantity and value (`quantity × cost price`) per product and warehouse.
- **FR-47:** Purchase report: total purchases per supplier for a date range, counting `Received` orders only.
- **FR-48:** Accounts receivable report: unpaid, partially paid, and overdue invoices by customer, with outstanding balance.
- **FR-49:** The system shall write an audit log entry for every create, update, delete, approval, and status change, containing user, action, entity, entity ID, old values, new values, and UTC timestamp. Passwords and tokens shall never be logged.
- **FR-50:** An Admin shall view and filter the audit log by user, entity, and date range. The audit log is append-only and cannot be edited or deleted through the API.

---

## 4. Data Requirements

All primary keys are `id`. All money columns use `decimal(18,2)` (`numeric(18,2)` in PostgreSQL). Soft-deletable tables have `is_active`. All tables have `created_at` (UTC, PostgreSQL `now()`).

| Entity | Fields and rules |
| :--- | :--- |
| **User** | `id`, `name`, `email` (unique), `password_hash`, `role`, `is_active`, `failed_login_count`, `lockout_end`, `created_at` |
| **Category** | `id`, `name` (unique), `description` |
| **Product** | `id`, `name`, `sku` (unique), `description` (optional), `category_id`, `unit_price`, `cost_price`, `unit`, `reorder_level`, `is_active`, `created_at` |
| **Warehouse** | `id`, `name`, `location`, `is_active` |
| **Stock** | `id`, `product_id`, `warehouse_id`, `quantity` (≥ 0, check constraint), `row_version` (optimistic concurrency via PostgreSQL `xmin` / EF `IsRowVersion()`). Unique `(product_id, warehouse_id)`. |
| **StockMovement** | `id`, `product_id`, `warehouse_id`, `quantity` (signed), `movement_type` (`RECEIPT` / `SALE` / `ADJUSTMENT` / `TRANSFER_OUT` / `TRANSFER_IN`), `reference_type`, `reference_id`, `reason` (for adjustments), `created_by`, `created_at` |
| **Supplier** | `id`, `name`, `email` (unique, optional), `phone`, `address`, `is_active` |
| **PurchaseOrder** | `id`, `po_number` (unique), `supplier_id`, `order_date`, `status` (`DRAFT` / `APPROVED` / `RECEIVED` / `CANCELLED`), `total_amount`, `created_by`, `approved_by`, `approved_at` |
| **PurchaseOrderItem** | `id`, `purchase_order_id`, `product_id`, `quantity`, `unit_cost`. Unique `(purchase_order_id, product_id)`. |
| **GoodsReceipt** | `id`, `purchase_order_id` (unique), `warehouse_id`, `received_by`, `received_at` |
| **Customer** | `id`, `name`, `email` (unique, optional), `phone`, `address`, `is_active` |
| **SalesOrder** | `id`, `so_number` (unique), `customer_id`, `warehouse_id`, `order_date`, `status` (`DRAFT` / `CONFIRMED` / `DELIVERED` / `CANCELLED`), `subtotal`, `discount`, `total_amount`, `created_by` |
| **SalesOrderItem** | `id`, `sales_order_id`, `product_id`, `quantity`, `unit_price`. Unique `(sales_order_id, product_id)`. |
| **Invoice** | `id`, `invoice_number` (unique), `sales_order_id` (unique), `issue_date`, `due_date`, `total_amount`, `status` (`UNPAID` / `PARTIALLY_PAID` / `PAID` / `CANCELLED`) |
| **Payment** | `id`, `invoice_id`, `amount`, `payment_date`, `method` (`CASH` / `BANK_TRANSFER` / `CARD` / `CHEQUE`), `recorded_by` |
| **Department** | `id`, `name` (unique) |
| **Employee** | `id`, `name`, `email` (unique), `phone`, `department_id`, `designation`, `hire_date`, `salary`, `is_active` |
| **Attendance** | `id`, `employee_id`, `date`, `status` (`PRESENT` / `ABSENT` / `ON_LEAVE`). Unique `(employee_id, date)`. |
| **AuditLog** | `id`, `user_id`, `action`, `entity_name`, `entity_id`, `old_values`, `new_values`, `timestamp` |

**Relationships:**
- `Category` `1–*` `Product`
- `Product` `1–*` `Stock`; `Warehouse` `1–*` `Stock`
- `Supplier` `1–*` `PurchaseOrder`; `PurchaseOrder` `1–*` `PurchaseOrderItem`; `PurchaseOrder` `1–1` `GoodsReceipt`
- `Customer` `1–*` `SalesOrder`; `SalesOrder` `1–*` `SalesOrderItem`; `SalesOrder` `1–1` `Invoice`
- `Invoice` `1–*` `Payment`
- `Department` `1–*` `Employee`; `Employee` `1–*` `Attendance`

---

## 5. Non-Functional Requirements

### Performance
- **NFR-1:** Standard read operations shall respond within 2 seconds under normal load (up to 50 concurrent users, 100,000 products).
- **NFR-1a:** Frequently filtered columns (SKU, status, dates, foreign keys) shall be indexed.

### Security
- **NFR-2:** Every request except login shall carry a valid JWT. Access tokens expire after a configured period (default 60 minutes).
- **NFR-3:** RBAC shall be enforced on the server for every endpoint; unauthorized access returns `403`, missing or invalid tokens return `401`.
- **NFR-3a:** Secrets (JWT key, connection strings) shall be kept outside source code (user secrets or environment variables). The API shall be served over HTTPS and CORS shall allow only configured origins.
- **NFR-3b:** Queries shall use EF Core parameterization to prevent SQL injection.

### Architecture and Maintainability
- **NFR-4:** The system shall separate controllers, services, and repositories. Controllers contain no business logic. DTOs are used instead of exposing entities.
- **NFR-5:** Monetary values shall use `decimal(18,2)`, never `float` or `double`.
- **NFR-6:** Multi-step operations (goods receipt, delivery, transfer, payment) shall run in one database transaction and roll back fully on failure.
- **NFR-7:** Concurrent updates to the same stock record shall be handled with optimistic concurrency (a row version column). On conflict the API returns `409` and the operation can be retried.
- **NFR-7a:** Document numbers (PO, SO, invoice) shall be generated safely under concurrency and never duplicated.

### API Quality
- **NFR-8:** Requests shall be validated (for example FluentValidation) and errors returned in one consistent format (RFC 7807 `ProblemDetails`) with correct status codes: `400`, `401`, `403`, `404`, `409`, `422`, `500`.
- **NFR-9:** All list endpoints shall support pagination, sorting, and filtering, with a maximum page size of 100.
- **NFR-10:** All endpoints shall be documented with OpenAPI (Swagger), including JWT authorization and example responses.
- **NFR-10a:** The API shall be versioned (for example `/api/v1/...`).

### Reliability and Operations
- **NFR-11:** Core business logic (stock updates, status transitions, invoice calculations, payment limits) shall be covered by automated unit tests, with at least 70% coverage on the service layer. Key flows shall also have integration tests.
- **NFR-12:** Database schema changes shall be managed with EF Core migrations, and sample seed data (an Admin user, roles, a few products) shall be provided.
- **NFR-13:** The system shall log errors and key events (Serilog or built-in logging) and expose a health-check endpoint.

---

## 6. Use Cases

| Use case | Actor | Main flow | Alternative / error flow |
| :--- | :--- | :--- | :--- |
| **Login** | Any user | Sends email and password, receives JWT. | Wrong credentials: `401`. Locked or inactive account: rejected. |
| **Manage Products and Stock** | Inventory Manager | Creates products, assigns categories, adjusts or transfers stock. | Negative result: rejected. Adjustment without reason: rejected. |
| **Create and Receive Purchase Order** | Purchase Officer, Purchase Manager | Officer creates PO; manager approves; officer records goods receipt; stock increases. | Creator tries to approve: rejected. PO not approved: receipt rejected. |
| **Create and Confirm Sales Order** | Sales Officer | Creates SO; system checks stock; order is confirmed. | Not enough stock: confirmation rejected. |
| **Deliver Sales Order** | Sales Officer | Marks order delivered; stock decreases; movements recorded. | Stock changed since confirmation: delivery rejected. |
| **Generate Invoice and Record Payment** | Sales Officer, Accountant | Invoice generated; payments recorded until `Paid`. | Overpayment: rejected. Second invoice: rejected. |
| **Manage Employees and Attendance** | HR Officer | Maintains employees, records daily attendance. | Duplicate attendance for a date: rejected. |
| **View Reports** | Authorized users | Selects a period and views sales, inventory, purchase, receivables reports. | Invalid date range: `400`. |
| **View Audit Log** | Admin | Filters log by user, entity, date. | Non-admin: `403`. |

*(A detailed use case diagram can be created separately.)*

---

## 7. Acceptance Criteria (examples)

1. Confirming a sales order for 10 units when only 8 are in stock returns an error and leaves the order in `Draft`.
2. Receiving an approved PO of 20 units increases stock by 20, creates one movement per line, and sets the status to `Received`.
3. A payment that would make the total exceed the invoice total is rejected and nothing is saved.
4. Two simultaneous deliveries of the last 5 units: one succeeds, the other fails with `409` or insufficient stock; stock never goes below zero.
5. A Sales Officer calling an Accountant-only endpoint receives `403`.
6. Every status change appears in the audit log with the acting user.

---

## Appendix A: Summary of Corrections from v1.0

| Issue in v1.0 | Fix in v2.0 |
| :--- | :--- |
| Author placeholder, no version or revision history | Added version block and revision history |
| Definitions and data tables were broken (misaligned rows) | Rebuilt as clean tables |
| "Approval rights" contradicted the one-role-per-user rule | Added Purchase Manager role; creator cannot approve own PO |
| Goods receipt referenced but had no entity | Added `GoodsReceipt` entity |
| Sales order had no warehouse, yet stock is per warehouse | Added `warehouse_id` to `SalesOrder` |
| No allowed status transitions | Defined transitions for PO, SO, and invoice |
| Stock check only at confirmation (overselling possible) | Re-check at delivery; reservation excluded and stated |
| Transfer used one movement type and one row | Signed quantities and `TRANSFER_OUT` / `TRANSFER_IN` pair |
| Invoice due date source and cancellation rules missing | Payment terms, overdue calculation, `Cancelled` status |
| Discount undefined | Defined fixed or percentage at order level with limits |
| Prices could change after ordering | Price snapshot copied into order lines |
| Reports did not say which orders count | Sales counts `Delivered`, purchases count `Received` |
| Audit log lacked old and new values | Added `old_values` and `new_values` |
| Concurrency requirement had no data support | Added `row_version` on `Stock` |
| Security details missing | Lockout, password rules, secrets, HTTPS, CORS, salary visibility |
| No validation, uniqueness, or cascade rules | Added per-requirement rules and unique constraints |
| No acceptance criteria, operations, or versioning | Added acceptance criteria, migrations, seed data, logging, API versioning |
