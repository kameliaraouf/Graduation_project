using GraduationProject.Data.Entities;


namespace GraduationProject.Repositories.Intefaces
{
    public interface IPharmacyRepo
    {
        Task<Pharmacy> GetPharmacyByIdAsync(int pharmacyId);
        Task<Pharmacy> GetPharmacyByEmailAsync(string email);

        Task<IEnumerable<Order>> GetAllOrdersByPharmacyIdAsync(int pharmacyId);
        Task UpdatePharmacyAsync(Pharmacy pharmacy);
        Task SaveChangesAsync();

    }
}
