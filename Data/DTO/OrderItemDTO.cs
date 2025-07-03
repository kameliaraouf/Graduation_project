using GraduationProject.Data.Entities;

namespace GraduationProject.Data.DTO
{
    public class OrderItemDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public string ProductImagePath { get; set; }


    }
}
