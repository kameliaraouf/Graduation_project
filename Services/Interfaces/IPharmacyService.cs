using GraduationProject.Data.DTO;
using GraduationProject.Data.Entities;

namespace GraduationProject.Services.Interfaces
{
    public interface IPharmacyService
    {
        Task<bool> PharmacyExistsByName(string name);
        Task<IEnumerable<string>> GetAllPharmacyNames();
        Task<string> RegisterPharmacyAsync(PharmacyRegisterDTO model);
        Task<bool> DeletePharmacyAsync(int pharmacyId);

        Task<Pharmacy> GetPharmacyByIdAsync(int id);
    }
}
