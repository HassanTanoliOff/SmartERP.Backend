namespace Core.Enums;

public enum StockMovementReferenceTypes
{
    UnKnownType = 0,
    PurchaseOrder = 1, // stock received from a supplier
    SalesOrder = 2, // stock sent to a customer
    Transfer = 3, // moved between warehouses
    Adjustment = 4, // manual correction (count, damage, loss)
    Return = 5, // customer or supplier return
    Opening = 6, // initial stock when the system starts
}
