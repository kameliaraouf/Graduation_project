using GraduationProject.Data.DTO;
using GraduationProject.Data.Enums;
using GraduationProject.Repositories.Interfaces;
using GraduationProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderRepository _orderRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderController(IOrderService orderService, IOrderRepository orderRepository, IWebHostEnvironment webHostEnvironment)
        {
            _orderService = orderService;
            _orderRepository = orderRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/Order - Get user's orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetUserOrders()
        {
            int userId = GetUserIdFromToken();
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        // GET: api/Order/confirmation/{code} - Get specific order by confirmation code
        [HttpGet("confirmation/{code}")]
        public async Task<ActionResult<OrderDTO>> GetOrderByConfirmation(string code)
        {
            int userId = GetUserIdFromToken();
            var order = await _orderService.GetOrderByConfirmationCodeAsync(code, userId);

            if (order == null)
            {
                return NotFound("Order not found or you don't have access to it");
            }

            return Ok(order);
        }

        // GET: api/Order/pharmacy - Get pharmacy's orders
        [HttpGet("pharmacy/{id}")]
        [Authorize(Roles = "Pharmacy")]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int id)
        {
            int pharmacyId = GetUserIdFromToken();
            var order = await _orderService.GetOrderByIdAsync( id,pharmacyId);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // GET: api/Order/all - Get all orders (pharmacy only)
        [HttpGet("all")]
        [Authorize(Roles = "Pharmacy")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // POST: api/Order - Create new order from cart
        [HttpPost]
        public async Task<ActionResult<OrderDTO>> CreateOrder(CreateOrderDTO createOrderDto)
        {
            int userId = GetUserIdFromToken();
            var order = await _orderService.CreateOrderFromCartAsync(userId, createOrderDto);

            if (order == null)
            {
                return BadRequest("Failed to create order. Cart might be empty.");
            }

            return CreatedAtAction(nameof(GetOrderByConfirmation), new { code = order.ConfirmationCode }, order);
        }

        // PUT: api/Order/confirmation/{code} - Update an order by confirmation code
        [HttpPut("confirmation/{code}")]
        public async Task<ActionResult<OrderDTO>> UpdateOrderByConfirmation(string code, UpdateOrderDTO updateOrderDto)
        {
            int userId = GetUserIdFromToken();
            var order = await _orderService.UpdateOrderByConfirmationAsync(userId, code, updateOrderDto);

            if (order == null)
            {
                return NotFound("Order not found, not owned by you, or can't be updated in its current state");
            }

            return Ok(order);
        }

        // DELETE: api/Order/confirmation/{code} - Delete an order by confirmation code
        [HttpDelete("confirmation/{code}")]
        public async Task<IActionResult> DeleteOrderByConfirmation(string code)
        {
            int userId = GetUserIdFromToken();
            var result = await _orderService.DeleteOrderByConfirmationAsync(code, userId);

            if (!result)
            {
                return NotFound("Order not found, not owned by you, or can't be deleted in its current state");
            }

            return NoContent();
        }

        // PUT: api/Order/status - Update order status (pharmacy only)
        [HttpPut("status")]
        [Authorize(Roles = "Pharmacy")]
        public async Task<ActionResult<OrderDTO>> UpdateOrderStatus([FromForm] UpdateOrderStatusDTO updateStatusDto)
        {
            try
            {
                string receiptImagePath = null;
                var file = Request.Form.Files.FirstOrDefault();

                int pharmacyId = GetUserIdFromToken();

                // Check if the order exists and belongs to the pharmacy
                var existingOrder = await _orderRepository.GetOrderByIdAsync(updateStatusDto.OrderID);
                if (existingOrder == null)
                    return NotFound("Order not found");

                bool isAssigned = await _orderRepository.IsOrderAssignedToPharmacyAsync(updateStatusDto.OrderID, pharmacyId);
                if (!isAssigned)
                    return Forbid("This order is not assigned to your pharmacy");

                // Determine new statuses
                bool statusChangingToDelivered = updateStatusDto.Status == OrderEnums.OrderStatus.Delivered;
                bool statusChangingToPaid = updateStatusDto.PaymentStatus == OrderEnums.PaymentStatus.Paid;

                // Validation: if Paid && Delivered => must upload receipt
                if (statusChangingToPaid && statusChangingToDelivered)
                {
                    if (file == null || file.Length == 0)
                        return BadRequest("You must upload a receipt image when marking the order as Paid and Delivered");

                    // Save the receipt image
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "receipts");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    receiptImagePath = "/receipts/" + uniqueFileName;
                }
                else
                {
                    // Validation: if not Paid & Delivered, uploading receipt is NOT allowed
                    if (file != null && file.Length > 0)
                    {
                        return BadRequest("Receipt image upload is only allowed when marking the order as Paid and Delivered");
                    }
                }

                // Perform the update
                var order = await _orderService.UpdateOrderStatusAsync(pharmacyId, updateStatusDto, receiptImagePath);
                if (order == null)
                    return BadRequest("Failed to update order status");

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        // Helper method to get user ID from token
        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            return int.Parse(userIdClaim.Value);
        }
    }
}