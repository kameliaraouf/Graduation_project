using GraduationProject.Data.Entities;
using GraduationProject.DTO;

namespace GraduationProject.Services.Interfaces
{
    public interface IAuthService
    {
        ////Task<string> AuthenticateAsync(string username, string password);
        // Task<object> LoginAsync(PharmacyLoginDTO loginDTO);

        string GenerateToken(Pharmacy pharmacy, string role);
        string GenerateAdminToken(User admin);
        string GenerateUserToken(User user);
    }
}
