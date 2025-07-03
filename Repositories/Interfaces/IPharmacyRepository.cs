using GraduationProject.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Repositories.Interfaces
{
    public interface IPharmacyRepository
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<string>> GetAllPharmacyNamesAsync();
        // Add to IPharmacyRepository
        Task<bool> ExistsByEmailAsync(string email);
        Task<Pharmacy> AddAsync(Pharmacy pharmacy);
        Task<Pharmacy> GetPharmacyByIdAsync(int id);
        void Remove(Pharmacy pharmacy);
    }
}
