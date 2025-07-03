using GraduationProject.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DTO
{
    public interface PaymentProcessDTO
    {
     
        public DateTime date { get; set; }
        public decimal TotalPrice { get; set; }
        string Status { get; set; }

       // [ForeignKey("Order")]
        public int OrderID { get; set; }

        //public Order Order { get; set; }
    }
}
