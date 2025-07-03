using GraduationProject.Data;
using GraduationProject.Data.Entities;
using GraduationProject.Data.Entities;
using GraduationProject.Repositories.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Repositories
{
    public class DiagnosisRepo : IDiagnosisRepo
    {

        private readonly ApplicationDbContext _context;
        public DiagnosisRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Diagnosis> AddAsync(Diagnosis diagnosis)
        {
            await _context.Diagnoses.AddAsync(diagnosis);
            await _context.SaveChangesAsync();
            return diagnosis;
        }

        public async Task<Diagnosis> GetByIdAsync(int diagnosisId)
        {
            return await _context.Diagnoses.FirstOrDefaultAsync(d => d.Id == diagnosisId);
        }

        public async Task<List<Diagnosis>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Diagnoses.Where(u => u.UserID == userId).ToListAsync();
        }
        public async Task DeleteAsync(int diagnosisId)
        {
            var diagnosis = await _context.Diagnoses.FirstOrDefaultAsync(d => d.Id == diagnosisId);
            if (diagnosis != null)
            {
                _context.Diagnoses.Remove(diagnosis);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int diagnosisId)
        {
            return await _context.Diagnoses.AnyAsync(d => d.Id == diagnosisId);
        }

    }
}
