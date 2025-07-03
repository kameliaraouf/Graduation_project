namespace GraduationProject.Data.Entities
{
    public class OrderItem
    {
        public int OrderID { get; set; }
        public Order order { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public int ProductID { get; set; }
        public Product product { get; set; }


    }
}
