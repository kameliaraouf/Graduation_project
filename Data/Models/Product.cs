using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using GraduationProject.Data.Models;

namespace GraduationProject.Data.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductCategory
    {
        SkincareProduct,
        HerbalProduct,
        Medicine
    }
    public class Product
    {
        [Key] // Specifies this property is the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get;set; }
        public string Name { get; set;}
        public string Description { get; set; }
        
        [Column(TypeName = "nvarchar(max)")]
        public string Img { get; set; }
      
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
      //  public string? Review { get; set; }
        public int Quantity { get; set; }
        public ProductCategory Category { get; set; }
        public string pharmacyName { get; set; }
        public double ?AverageRating { get; set; }

        public  int UserID { get; set; }
        public User user { get; set; }
      
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<ProductPharmacy> productPharmacy { get; set; }    
        public ICollection<ProductReview> Reviews { get; set; }

    }
}
