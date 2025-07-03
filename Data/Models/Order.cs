using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GraduationProject.Data.Entities
{

    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public User user { get; set; }

        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }

        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Governorate { get; set; }
        public string Phone { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? receiptIMG { get; set; }

        public string? ConfirmationCode { get; set; }


        public int PharmacyID { get; set; }
        public Pharmacy Pharmacy { get; set; }
        public ICollection<OrderItem>OrderItems { get; set; }
        public Payment payment {get;set;}
    }
}
