namespace Core.Enums;

public enum UserRoles
{
    Anonymous = 0,

    // Manages users, roles, settings, audit log; access to
    // all modules; can approve purchase orders.
    Admin = 1,

    // Manages categories, products, warehouses, stock,
    // adjustments, transfers.
    InventoryManager = 2,

    // Manages suppliers; creates and edits purchase
    // orders; records goods receipts.
    PurchaseOfficer = 3,

    // Approves or rejects purchase orders
    PurchaseManager = 4,

    // Manages customers and sales orders; generates
    // invoices.
    SalesOfficer = 5,

    // Records payments; views financial reports.
    Accountant = 6,

    // Manages departments, employees, and
    // attendance.
    HROfficer = 7,
    SuperAdmin = 8,
}
