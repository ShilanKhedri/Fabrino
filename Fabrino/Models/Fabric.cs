public class Fabric
{
    public int FabricID { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public string Material { get; set; }
    public decimal Width { get; set; }
    public decimal Quantity { get; set; }
    public decimal PricePerMeter { get; set; }
    public int SupplierID { get; set; }
    public Supplier Supplier { get; set; }
}
