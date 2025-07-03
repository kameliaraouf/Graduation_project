using GraduationProject.Data.Entities;
using GraduationProject.DTO;


namespace GraduationProject.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<(bool IsSuccess, string Message)> ProcessPaymentAsync(PaymentProcessDTO paymentDto);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<Payment> GetPaymentByOrderIdAsync(int orderId);
    }
}
