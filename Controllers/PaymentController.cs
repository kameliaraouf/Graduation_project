using GraduationProject.Data;
using GraduationProject.DTO;
using GraduationProject.Data.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Services.Interfaces;
using GraduationProject.Data.Entities;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] MyPaymentDTO dto)
        {
            var order = await _context.Orders.FindAsync(dto.OrderID);
            if (order == null)
                return NotFound("Order not found.");

            var payment = new Payment
            {
                OrderID = dto.OrderID,
                Status = dto.Status,
                date = dto.date ?? DateTime.UtcNow,
                TotalPrice = order.TotalPrice // assuming order has TotalPrice
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment created", payment.Id });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetAllPayments()
        {
            var payments = await _context.Payments
                .Include(p => p.Order)
                .ToListAsync();

            return Ok(payments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
                return NotFound("Payment not found.");

            return Ok(payment);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] MyPaymentDTO dto)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound("Payment not found.");

            payment.Status = dto.Status;
            payment.date = dto.date ?? payment.date;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment updated" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound("Payment not found.");

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Payment deleted" });
        }


    }
}
