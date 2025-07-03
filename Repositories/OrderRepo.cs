using GraduationProject.Data;
using GraduationProject.Data.Entities;
using GraduationProject.Data.Entities;
using GraduationProject.Repositories.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Repositories
{
    public class OrderRepo:IOrderRepo
    {
        private readonly ApplicationDbContext _context;
        public OrderRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderIdAsync(int Id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderID == Id);
        }
    }
}
