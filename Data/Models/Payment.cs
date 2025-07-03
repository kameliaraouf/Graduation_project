namespace GraduationProject.Data.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime date { get; set; }
        public decimal TotalPrice{get;set;}
        public string Status { get; set; }
        public int OrderID { get; set; }
        public Order Order { get; set; }
    }
}
