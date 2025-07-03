using GraduationProject.Data.Entities;


namespace GraduationProject.Repositories.Intefaces
{
    public interface IDiagnosisRepo
    {
        Task<Diagnosis> AddAsync(Diagnosis diagnosis);
        Task<Diagnosis> GetByIdAsync(int diagnosisId);
        Task<List<Diagnosis>> GetAllByUserIdAsync(int userId);
        Task DeleteAsync(int diagnosisId);
        Task<bool> ExistsAsync(int diagnosisId);
    }
}
