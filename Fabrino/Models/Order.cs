public class Order
{
    public int OrderID { get; set; }
    public int CustomerID { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal? TotalAmount { get; set; }


    //  ارتباط با مشتری
    public Customer Customer { get; set; }

    //  لیست اقلام سفارش
    public List<OrderItem> OrderItems { get; set; }
}
