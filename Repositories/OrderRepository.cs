using GraduationProject.Data;
using GraduationProject.Data.Entities;
using GraduationProject.Data.Enums;
using GraduationProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationProject.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.user)
                .Include(o => o.Pharmacy)
                .Include(o => o.payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.product)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }

        public async Task<Order> GetOrderByConfirmationCodeAsync(string confirmationCode)
        {
            return await _context.Orders
                .Include(o => o.user)
                .Include(o => o.Pharmacy)
                .Include(o => o.payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.product)
                .FirstOrDefaultAsync(o => o.ConfirmationCode == confirmationCode);
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.user)
                .Include(o => o.Pharmacy)
                .Include(o => o.payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.product)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.Date)
                .ToListAsync();
        }

        public async Task<List<Order>> GetPharmacyOrdersAsync(int pharmacyId)
        {
            return await _context.Orders
                .Include(o => o.user)
                .Include(o => o.Pharmacy)
                .Include(o => o.payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.product)
                .Where(o => o.PharmacyID == pharmacyId)
                .OrderByDescending(o => o.Date)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.user)
                .Include(o => o.Pharmacy)
                .Include(o => o.payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.product)
                .OrderByDescending(o => o.Date)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderFromCartAsync(int userId, string firstName, string lastName, string phone,
                                                        string city, string street, string governorate, int pharmacyId, string confirmationCode)
        {
            // Get user's cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return null; // Cart doesn't exist or is empty
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create new order
                var order = new Order
                {
                    UserID = userId,
                    Date = DateTime.Now,
                    TotalPrice = cart.TotalPrice,
                    Status = OrderEnums.OrderStatus.NotShipped.ToString(),
                    PaymentStatus = OrderEnums.PaymentStatus.NotPaid.ToString(),
                    City = city,
                    Street = street,
                    Governorate = governorate,
                    Phone = phone,
                    FirstName = firstName,
                    LastName = lastName,
                    PharmacyID = pharmacyId,
                    receiptIMG = null,
                    ConfirmationCode = confirmationCode
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Create payment record
                var payment = new Payment
                {
                    OrderID = order.OrderID,
                    TotalPrice = cart.TotalPrice,
                    date = DateTime.Now,
                    Status = OrderEnums.PaymentStatus.NotPaid.ToString()
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Create order items from cart items
                var orderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    OrderID = order.OrderID,
                    ProductID = ci.ProductID,
                    quantity = ci.Quantity,
                    price = ci.price
                }).ToList();

                _context.OrderItems.AddRange(orderItems);

                // Update product quantities
                foreach (var cartItem in cart.CartItems)
                {
                    var product = await _context.products.FindAsync(cartItem.ProductID);
                    if (product != null)
                    {
                        product.Quantity -= cartItem.Quantity;
                    }
                }

                // Clear the cart
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.TotalItems = 0;
                cart.TotalPrice = 0;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return the complete order
                return await GetOrderByIdAsync(order.OrderID);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order> GetOrderByDateAndConfirmationAsync(int userId,  string confirmationCode)
        {
            return await _context.Orders
                .Where(o => o.UserID == userId  &&
                       o.ConfirmationCode == confirmationCode)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null)
            {
                return false;
            }

            // Only allow deletion if order is not shipped or delivered
            if (order.Status != OrderEnums.OrderStatus.NotShipped.ToString())
            {
                return false;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Return products to inventory
                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _context.products.FindAsync(orderItem.ProductID);
                    if (product != null)
                    {
                        product.Quantity += orderItem.quantity;
                    }
                }

                // Delete payment
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderID == orderId);
                if (payment != null)
                {
                    _context.Payments.Remove(payment);
                }

                // Delete order items and order
                _context.OrderItems.RemoveRange(order.OrderItems);
                _context.Orders.Remove(order);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderEnums.OrderStatus status, OrderEnums.PaymentStatus paymentStatus, string receiptImagePath = null)
        {
            var order = await _context.Orders
                .Include(o => o.payment)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null)
            {
                return false;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                order.Status = status.ToString();
                order.PaymentStatus = paymentStatus.ToString();

                // Update receipt image path if provided
                if (!string.IsNullOrEmpty(receiptImagePath))
                {
                    order.receiptIMG = receiptImagePath;
                }

                if (order.payment != null)
                {
                    order.payment.Status = paymentStatus.ToString();
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> IsOrderOwnedByUserAsync(int orderId, int userId)
        {
            return await _context.Orders.AnyAsync(o => o.OrderID == orderId && o.UserID == userId);
        }

        public async Task<bool> IsOrderAssignedToPharmacyAsync(int orderId, int pharmacyId)
        {
            return await _context.Orders.AnyAsync(o => o.OrderID == orderId && o.PharmacyID == pharmacyId);
        }
    }
}