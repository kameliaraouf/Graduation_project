using GraduationProject.Data.Entities;

namespace GraduationProject.Services.Interfaces
{
    public interface IDiagnosisService
    {
        Task<(int diagnosisId, string result)> UploadAndProcessImageAsync(IFormFile file, int userId);
        Task<Diagnosis> GetDiagnosisDetailsAsync(int diagnosisId);
        Task<List<Diagnosis>> GetAllDiagnosesForUserAsync(int userId);
        Task DeleteDiagnosisAsync(int diagnosisId);
    }
}
