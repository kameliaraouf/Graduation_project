using GraduationProject.Data.Entities;


namespace GraduationProject.Repositories.Intefaces
{
    public interface IPaymentRepo
    {

        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<Payment> GetPaymentByOrderIdAsync(int orderId);
      //  Task<Order> GetOrderByIdAsync(int orderId);
        Task AddPaymentAsync(Payment payment);
       // Task UpdateOrderAsync(Order order);
        Task SaveChangesAsync();
    }
}
