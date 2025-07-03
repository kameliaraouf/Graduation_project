using GraduationProject.Data.DTO;
using GraduationProject.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GraduationProject.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate);

        Task<User> GetByUsernameAsync(String UserName);
        Task<User> GetByIdAsync(int userId);
        Task AddAsync(User user);
        Task<bool> SaveChangesAsync();
        void Update(User user);
        void Delete(User user);
       




    }
}
