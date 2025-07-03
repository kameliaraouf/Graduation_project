using GraduationProject.Data;
using GraduationProject.Data.Entities;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Repositories.Interfaces;
namespace GraduationProject.Repositories
{
    public class PharmacyRepository:IPharmacyRepository
    {
        private readonly ApplicationDbContext _context;

        public PharmacyRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.pharmacies.AnyAsync(p => p.Name == name);
        }
        public async Task<IEnumerable<string>> GetAllPharmacyNamesAsync()
        {
            return await _context.pharmacies
                .Select(p => p.Name)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.pharmacies.AnyAsync(p => p.Email == email);
        }

        public async Task<Pharmacy> AddAsync(Pharmacy pharmacy)
        {
            await _context.pharmacies.AddAsync(pharmacy);
            await _context.SaveChangesAsync();
            return pharmacy;
        }
        public async Task<Pharmacy> GetPharmacyByIdAsync(int id)
        {
            return await _context.pharmacies
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public void Remove(Pharmacy pharmacy)
        {
            _context.pharmacies.Remove(pharmacy);
            _context.SaveChanges();
        }

    }
}
