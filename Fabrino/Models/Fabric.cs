using Fabrino.Models;

/// <summary>
/// Represents a fabric product in the inventory system
/// </summary>
public class Fabric
{
    /// <summary>Unique identifier for the fabric</summary>
    public int FabricID { get; set; }

    /// <summary>Name or description of the fabric</summary>
    public string Name { get; set; }

    /// <summary>Color of the fabric</summary>
    public string Color { get; set; }

    /// <summary>Material composition of the fabric</summary>
    public string Material { get; set; }

    /// <summary>Width of the fabric in meters</summary>
    public decimal Width { get; set; }

    /// <summary>Available quantity in stock</summary>
    public decimal Quantity { get; set; }

    /// <summary>Price per meter of fabric</summary>
    public decimal PricePerMeter { get; set; }

    /// <summary>ID of the supplier providing this fabric</summary>
    public int SupplierID { get; set; }

    /// <summary>Navigation property to the supplier</summary>
    public Supplier Supplier { get; set; }

    /// <summary>List of purchase order items containing this fabric</summary>
    public List<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
}
