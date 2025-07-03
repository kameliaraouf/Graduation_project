using GraduationProject.Data.Entities;

namespace GraduationProject.DTO
{
    public class DiagnosisDTO
    {
      
        public string Result { get; set; }
      
        public decimal Accuracy { get; set; }
        public DateTime Date { get; set; }
        public string img { get; set; }

        // public int UserID { get; set; }
        // public User user { get; set; }
    }
}
