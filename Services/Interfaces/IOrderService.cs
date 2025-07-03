using GraduationProject.Data.DTO;
using GraduationProject.Data.Models;
using GraduationProject.DTO;
using GraduationProject.Repositories.Interfaces;
using System.Threading.Tasks;
namespace GraduationProject.Services.Interfaces
{
    public interface IOrderService
    {


        Task<OrderDTO> GetOrderByIdAsync(int orderId, int userId);
        Task<OrderDTO> GetOrderByConfirmationCodeAsync(string confirmationCode, int userId);
        Task<List<OrderDTO>> GetUserOrdersAsync(int userId);
        Task<List<OrderDTO>> GetPharmacyOrdersAsync(int pharmacyId);
        Task<List<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO> CreateOrderFromCartAsync(int userId, CreateOrderDTO createOrderDto);
        Task<OrderDTO> UpdateOrderAsync(int userId, UpdateOrderDTO updateOrderDto);
        Task<OrderDTO> UpdateOrderByConfirmationAsync(int userId, string confirmationCode, UpdateOrderDTO updateOrderDto);
        Task<bool> DeleteOrderAsync(int orderId, int userId);
        Task<bool> DeleteOrderByConfirmationAsync(string confirmationCode, int userId);
        Task<OrderDTO> UpdateOrderStatusAsync(int pharmacyId, UpdateOrderStatusDTO updateStatusDto, string receiptImagePath = null);



    }
}