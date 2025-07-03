using GraduationProject.Data.DTO;
using GraduationProject.Data.Entities;
using GraduationProject.Data.Enums;
using GraduationProject.Repositories.Interfaces;
using GraduationProject.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationProject.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int orderId, int userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null || (order.UserID != userId && !await _orderRepository.IsOrderAssignedToPharmacyAsync(orderId, userId)))
            {
                return null;
            }

            return MapOrderToDto(order);
        }

        public async Task<OrderDTO> GetOrderByConfirmationCodeAsync(string confirmationCode, int userId)
        {
            var order = await _orderRepository.GetOrderByConfirmationCodeAsync(confirmationCode);

            if (order == null || (order.UserID != userId && !await _orderRepository.IsOrderAssignedToPharmacyAsync(order.OrderID, userId)))
            {
                return null;
            }

            return MapOrderToDto(order);
        }

        public async Task<List<OrderDTO>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetUserOrdersAsync(userId);
            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<List<OrderDTO>> GetPharmacyOrdersAsync(int pharmacyId)
        {
            var orders = await _orderRepository.GetPharmacyOrdersAsync(pharmacyId);
            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<List<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<OrderDTO> CreateOrderFromCartAsync(int userId, CreateOrderDTO createOrderDto)
        {
            var random = new Random();
            var confirmationCode = random.Next(100000, 999999).ToString(); // 6-digit random number

            var order = await _orderRepository.CreateOrderFromCartAsync(
                userId,
                createOrderDto.FirstName,
                createOrderDto.LastName,
                createOrderDto.Phone,
                createOrderDto.City,
                createOrderDto.Street,
                createOrderDto.Governorate,
                1,
                confirmationCode // Default to pharmacy ID 1, you might want to make this dynamic
            );

            if (order == null)
            {
                return null;
            }

            return MapOrderToDto(order);
        }

        public async Task<OrderDTO> UpdateOrderAsync(int userId, UpdateOrderDTO updateOrderDto)
        {
            // Get the order using the date and confirmation code
            var order = await _orderRepository.GetOrderByDateAndConfirmationAsync(
                userId,

                updateOrderDto.ConfirmationCode);

            if (order == null)
            {
                return null;
            }

            // Only allow updates if order is not shipped or delivered
            if (order.Status != OrderEnums.OrderStatus.NotShipped.ToString())
            {
                return null;
            }

            // Update order details
            order.FirstName = updateOrderDto.FirstName;
            order.LastName = updateOrderDto.LastName;
            order.Phone = updateOrderDto.Phone;
            order.City = updateOrderDto.City;
            order.Street = updateOrderDto.Street;
            order.Governorate = updateOrderDto.Governorate;

            bool result = await _orderRepository.UpdateOrderAsync(order);
            if (!result)
            {
                return null;
            }

            return MapOrderToDto(order);
        }

        public async Task<OrderDTO> UpdateOrderByConfirmationAsync(int userId, string confirmationCode, UpdateOrderDTO updateOrderDto)
        {
            // Get the order using the confirmation code
            var order = await _orderRepository.GetOrderByConfirmationCodeAsync(confirmationCode);

            if (order == null || order.UserID != userId)
            {
                return null;
            }

            // Only allow updates if order is not shipped or delivered
            if (order.Status != OrderEnums.OrderStatus.NotShipped.ToString())
            {
                return null;
            }

            // Update order details
            order.FirstName = updateOrderDto.FirstName;
            order.LastName = updateOrderDto.LastName;
            order.Phone = updateOrderDto.Phone;
            order.City = updateOrderDto.City;
            order.Street = updateOrderDto.Street;
            order.Governorate = updateOrderDto.Governorate;

            bool result = await _orderRepository.UpdateOrderAsync(order);
            if (!result)
            {
                return null;
            }

            return MapOrderToDto(order);
        }

        public async Task<bool> DeleteOrderAsync(int orderId, int userId)
        {
            if (!await _orderRepository.IsOrderOwnedByUserAsync(orderId, userId))
            {
                return false;
            }

            return await _orderRepository.DeleteOrderAsync(orderId);
        }

        public async Task<bool> DeleteOrderByConfirmationAsync(string confirmationCode, int userId)
        {
            var order = await _orderRepository.GetOrderByConfirmationCodeAsync(confirmationCode);

            if (order == null || order.UserID != userId)
            {
                return false;
            }

            return await _orderRepository.DeleteOrderAsync(order.OrderID);
        }

        public async Task<OrderDTO> UpdateOrderStatusAsync(int pharmacyId, UpdateOrderStatusDTO updateStatusDto, string receiptImagePath = null)
        {
            if (!await _orderRepository.IsOrderAssignedToPharmacyAsync(updateStatusDto.OrderID, pharmacyId))
            {
                return null;
            }

            var order = await _orderRepository.GetOrderByIdAsync(updateStatusDto.OrderID);
            if (order == null)
            {
                return null;
            }

            // Determine if a receipt image is required
            bool receiptRequired = false;

            // Receipt is required if:
            // 1. User is trying to mark as Delivered (and it wasn't already Delivered)
            // 2. User is trying to mark as Paid (and it wasn't already Paid)
            bool willBeDelivered = updateStatusDto.Status == OrderEnums.OrderStatus.Delivered;
            bool willBePaid = updateStatusDto.PaymentStatus == OrderEnums.PaymentStatus.Paid;

            bool isCurrentlyDelivered = order.Status == OrderEnums.OrderStatus.Delivered.ToString();
            bool isCurrentlyPaid = order.PaymentStatus == OrderEnums.PaymentStatus.Paid.ToString();

            // If changing to Delivered or Paid and not already in that state, receipt is required
            if ((willBeDelivered && !isCurrentlyDelivered) || (willBePaid && !isCurrentlyPaid))
            {
                receiptRequired = true;
            }

            // Check if receipt exists (either already in DB or provided in current request)
            bool hasReceipt = !string.IsNullOrEmpty(order.receiptIMG) || !string.IsNullOrEmpty(receiptImagePath);

            // If receipt is required but not provided, return failure
            if (receiptRequired && !hasReceipt)
            {
                return null; // This will trigger the appropriate error message in the controller
            }

            // Update order status
            bool result = await _orderRepository.UpdateOrderStatusAsync(
                updateStatusDto.OrderID,
                updateStatusDto.Status,
                updateStatusDto.PaymentStatus,
                receiptImagePath
            );

            if (!result)
            {
                return null;
            }

            var updatedOrder = await _orderRepository.GetOrderByIdAsync(updateStatusDto.OrderID);
            return MapOrderToDto(updatedOrder);
        }


        // Helper method to map Order entity to OrderDTO
        private OrderDTO MapOrderToDto(Order order)
        {
            if (order == null)
            {
                return null;
            }

            OrderEnums.OrderStatus orderStatus;
            OrderEnums.PaymentStatus paymentStatus;

            // Properly parse the status strings from the database
            if (!Enum.TryParse<OrderEnums.OrderStatus>(order.Status, out orderStatus))
            {
                orderStatus = OrderEnums.OrderStatus.NotShipped;
            }

            if (!Enum.TryParse<OrderEnums.PaymentStatus>(order.PaymentStatus, out paymentStatus))
            {
                paymentStatus = OrderEnums.PaymentStatus.NotPaid;
            }

            return new OrderDTO
            {
                OrderID = order.OrderID, // Include for internal use but hide from client
                ConfirmationCode = order.ConfirmationCode,
                UserID = order.UserID,
                UserName = order.user?.UserName,
                Date = order.Date,
                TotalPrice = order.TotalPrice,
                Status = orderStatus,
                PaymentStatus = paymentStatus,
                City = order.City,
                Street = order.Street,
                Governorate = order.Governorate,
                Phone = order.Phone,
                FirstName = order.FirstName,
                LastName = order.LastName,
                ReceiptIMG = order.receiptIMG,
                PharmacyID = order.PharmacyID,
                PharmacyName = order.Pharmacy?.Name,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemDTO
                {
                    ProductID = oi.ProductID,
                    ProductName = oi.product?.Name,
                    Price = oi.price,
                    Quantity = oi.quantity,
                    Subtotal = oi.price * oi.quantity,
                    ProductImagePath = oi.product?.Img,

                }).ToList(),

                Payment = order.payment != null ? new PaymentOrderDTO
                {
                    PaymentID = order.payment.Id,
                    Amount = order.payment.TotalPrice,
                    Date = order.payment.date,
                    Status = order.payment.Status
                } : null
            };
        }
    }
}