public class OrderItem
{
    public int OrderItemID { get; set; }
    public int OrderID { get; set; }
    public int FabricID { get; set; }
    public decimal Quantity { get; set; }
    public decimal PricePerMeter { get; set; }

    // محاسبه مجموع قیمت بر اساس متراژ و قیمت هر متر
    public decimal TotalPrice => Quantity * PricePerMeter;

    // Optional: ارتباط با پارچه
    public Fabric Fabric { get; set; }
}
