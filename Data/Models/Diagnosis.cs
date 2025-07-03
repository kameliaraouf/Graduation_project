namespace GraduationProject.Data.Entities
{
    public class Diagnosis
    {
        public int Id { get; set; }
        public string img { get; set; }
        public string Result { get; set; }
        public DateTime Date { get; set; }
        public decimal Accuracy { get; set; }
        public int UserID { get; set; }
        public User user { get;set; }
    }
}
