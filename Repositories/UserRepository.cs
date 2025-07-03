using GraduationProject.Data.Entities;
using GraduationProject.Data;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Repositories.Interfaces;
using System.Linq.Expressions;

namespace GraduationProject.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

      
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.users
                .FirstOrDefaultAsync(u => u.UserName == username);
        }
        public async Task<User> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.users.FirstOrDefaultAsync(predicate);
        }

       

        public async Task AddAsync(User user)
        {
            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<User> GetByIdAsync(int userId)
        {
            return await _context.users.FindAsync(userId);
        }
        public void Update(User user)
        {
            _context.users.Update(user);
        }

        public void Delete(User user)
        {
            _context.users.Remove(user);
        }

      



    }
}
