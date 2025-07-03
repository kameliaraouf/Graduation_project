using GraduationProject.Data.Entities;

namespace GraduationProject.Data.DTO
{
    public class ProductResponseDTO
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Img { get; set; }
        public decimal Price { get; set; }
       // public string Review { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public string PharmacyName { get; set; }
        public double AverageRating { get; set; }
        public List<ReviewDetailDTO> Reviews { get; set; } = new List<ReviewDetailDTO> ();
    }
}

