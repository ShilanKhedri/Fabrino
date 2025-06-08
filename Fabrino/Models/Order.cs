/// <summary>
/// Represents a customer order in the system
/// </summary>
public class Order
{
    /// <summary>Unique identifier for the order</summary>
    public int OrderID { get; set; }

    /// <summary>ID of the customer who placed the order</summary>
    public int CustomerID { get; set; }

    /// <summary>Date when the order was placed</summary>
    public DateTime? OrderDate { get; set; }

    /// <summary>Total amount of the order</summary>
    public decimal? TotalAmount { get; set; }

    /// <summary>Navigation property to the customer</summary>
    public Customer Customer { get; set; }

    /// <summary>Collection of items in this order</summary>
    public List<OrderItem> OrderItems { get; set; }
}
