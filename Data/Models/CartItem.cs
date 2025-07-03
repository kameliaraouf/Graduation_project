using System.Net.Http.Headers;

namespace GraduationProject.Data.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartID { get; set; }
        public Cart cart { get;set; }
        public int Quantity { get; set; }
        public decimal price { get; set; }
        public int ProductID { get; set; }
        public Product product { get; set; }
    }
}
