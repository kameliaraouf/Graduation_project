using GraduationProject.Data.Entities;
using GraduationProject.Data.Entities;

namespace GraduationProject.Repositories.Intefaces
{
    public interface IOrderRepo
    {
        Task<Order> GetOrderIdAsync(int PaymentId);

    }
}
