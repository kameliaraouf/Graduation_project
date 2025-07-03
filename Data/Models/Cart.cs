namespace GraduationProject.Data.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public User user { get; set; }
        public int UserID { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
