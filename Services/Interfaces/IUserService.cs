using GraduationProject.Data.DTO;
using GraduationProject.Data.Entities;
using System.Linq.Expressions;

namespace GraduationProject.Services.Interfaces
{
    public interface IUserService
    {
        
        Task<string> RegisterUserAsync(UserRegisterDTO registerDTO);
        Task<string> LoginUserAsync(UserLoginDTO loginDTO);
        Task<string> LoginAdminAsync(AdminDTO adminDTO);
      
        Task<User> GetUserByIdAsync(int userId);
        Task<UserDTO> UpdateUserAsync(int userId, UserEditDTO editDTO);
        Task<bool> DeleteUserAsync(int userId);

       
      


    }
}
