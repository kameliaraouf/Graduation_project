using GraduationProject.Data.Entities;

using GraduationProject.Repositories.Intefaces;
using GraduationProject.Services.Interfaces;

namespace GraduationProject.Services
{
    public class DiagnosisService:IDiagnosisService
    {
        private readonly IDiagnosisRepo _diagnosisRepo;

        public DiagnosisService(IDiagnosisRepo diagnosisRepo)
        {
            _diagnosisRepo = diagnosisRepo;

            
        }
       
            public async Task<(int diagnosisId, string result)> UploadAndProcessImageAsync(IFormFile file, int userId)
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("No file uploaded");

                var filepath = Path.GetTempFileName();

                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);

                    // Here you would call your ML model
                    var result = "Eczema"; // Replace with actual ML model call

                    var diagnosis = new Diagnosis
                    {
                        UserID = userId,
                        img = filepath,
                        Result = result,
                        Date = DateTime.UtcNow
                    };

                    var savedDiagnosis = await _diagnosisRepo.AddAsync(diagnosis);
                    return (savedDiagnosis.Id, result);
                }


            }
        public async Task<Diagnosis> GetDiagnosisDetailsAsync(int diagnosisId)
        {
            var diagnosis = await _diagnosisRepo.GetByIdAsync(diagnosisId);
            if (diagnosis == null)
                throw new KeyNotFoundException($"Diagnosis with ID {diagnosisId} not found");

            return diagnosis;
        }
        public async Task<List<Diagnosis>> GetAllDiagnosesForUserAsync(int userId)
        {
            return await _diagnosisRepo.GetAllByUserIdAsync(userId);
        }
        public async Task DeleteDiagnosisAsync(int diagnosisId)
        {
            if (!await _diagnosisRepo.ExistsAsync(diagnosisId))
                throw new KeyNotFoundException($"Diagnosis with ID {diagnosisId} not found");

            await _diagnosisRepo.DeleteAsync(diagnosisId);
        }
    }
}
