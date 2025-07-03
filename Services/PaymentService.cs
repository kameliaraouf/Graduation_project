using GraduationProject.DTO;
using GraduationProject.Data.Entities;
using GraduationProject.Repositories.Intefaces;
using GraduationProject.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace GraduationProject.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepo _paymentRepo;
        private readonly IOrderRepo _orderRepo;

        public PaymentService(IPaymentRepo paymentRepo, IOrderRepo orderRepo)
        {
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
        }

        public async Task<(bool IsSuccess, string Message)> ProcessPaymentAsync(PaymentProcessDTO paymentDto)
        {
            var order = await _orderRepo.GetOrderIdAsync(paymentDto.OrderID);

            if (order == null)
                return (false, "Order Not Found");

            decimal totalAmount = order.OrderItems.Sum(item => item.price * item.quantity);

            if (paymentDto.TotalPrice != totalAmount)
                return (false, "Payment amount does not match the order total");

            var payment = new Payment
            {
                OrderID = paymentDto.OrderID,
                TotalPrice = paymentDto.TotalPrice,
                date = paymentDto.date,
                Status = "Pending"
            };

            order.Status = "Paid";

            await _paymentRepo.AddPaymentAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            return (true, "Payment processed successfully");
        }
        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return await _paymentRepo.GetPaymentByIdAsync(paymentId);
        }

        public async Task<Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _paymentRepo.GetPaymentByOrderIdAsync(orderId);
        }
    }
}

