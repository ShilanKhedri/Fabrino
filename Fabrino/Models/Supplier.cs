public class Supplier
{
    public int SupplierID { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }

    // Optional: لیست پارچه‌هایی که از این تأمین‌کننده خریداری شده
    public List<Fabric> Fabrics { get; set; }
}
