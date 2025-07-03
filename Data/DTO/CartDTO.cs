using GraduationProject.Data.Entities;

namespace GraduationProject.DTO
{
    public class CartDTO
    {

        public int Id { get; set; }
        public int userid { get; set; }

        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
    }
}
