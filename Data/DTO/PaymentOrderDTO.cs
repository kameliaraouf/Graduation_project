using GraduationProject.Data.Entities;
namespace GraduationProject.Data.DTO
{
    public class PaymentOrderDTO
    {
        public int PaymentID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}

