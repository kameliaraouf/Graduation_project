using GraduationProject.Data;
using GraduationProject.Data.Entities;

using GraduationProject.Repositories.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Repositories
{
    public class PharmacyRepo:IPharmacyRepo
    {
        private readonly ApplicationDbContext _context;

        public PharmacyRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Pharmacy> GetPharmacyByIdAsync(int pharmacyId)
        {
            return await _context.pharmacies.FirstOrDefaultAsync(p => p.Id == pharmacyId);
        }
        public async Task<Pharmacy> GetPharmacyByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.pharmacies.FirstOrDefaultAsync(p => p.Email == email);
        }
        public async Task<IEnumerable<Order>> GetAllOrdersByPharmacyIdAsync(int pharmacyId)
        {
            return await _context.Orders
                .Where(o => o.PharmacyID == pharmacyId)
                .ToListAsync();
        }

        public async Task UpdatePharmacyAsync(Pharmacy pharmacy)
        {
            _context.pharmacies.Update(pharmacy);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        
    }
}
