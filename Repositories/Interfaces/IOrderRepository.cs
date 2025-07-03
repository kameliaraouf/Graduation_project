using GraduationProject.Data.DTO;
using GraduationProject.Data.Entities;
using GraduationProject.Data.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GraduationProject.Data.Enums.OrderEnums;

namespace GraduationProject.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<Order> GetOrderByConfirmationCodeAsync(string confirmationCode);
        Task<List<Order>> GetUserOrdersAsync(int userId);
        Task<List<Order>> GetPharmacyOrdersAsync(int pharmacyId);
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order> CreateOrderFromCartAsync(int userId, string firstName, string lastName, string phone,
                                         string city, string street, string governorate, int pharmacyId, string confirmationCode);
        Task<Order> GetOrderByDateAndConfirmationAsync(int userId, string confirmationCode);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderEnums.OrderStatus status, OrderEnums.PaymentStatus paymentStatus, string receiptImagePath = null);
        Task<bool> IsOrderOwnedByUserAsync(int orderId, int userId);
        Task<bool> IsOrderAssignedToPharmacyAsync(int orderId, int pharmacyId);


    }
}