namespace GraduationProject.Data.Models
{
    public class ModelResponse
    {
       
        public Dictionary<string, Prediction> Predictions { get; set; }
        public List<string> PredictedClasses { get; set; }

      
    }
    public class Prediction
    {
        public double Confidence { get; set; }
        public int ClassId { get; set; }
    }
}
