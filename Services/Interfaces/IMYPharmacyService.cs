using GraduationProject.Data.Entities;
using GraduationProject.DTO;


namespace GraduationProject.Services.Interfaces
{
    public interface IMYPharmacyService
    {
        Task<Pharmacy> GetProfileAsync(int pharmacyId);
        Task<IEnumerable<Order>> GetAllOrdersAsync(int pharmacyId);
        Task<string> UpdateProfileAsync(int pharmacyId, PharmacyLoginDTO profileDto);
    }
}
