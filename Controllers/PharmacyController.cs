 using Azure.Core;
using GraduationProject.Data;
using GraduationProject.DTO;
using GraduationProject.Data.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GraduationProject.Services.Interfaces;
using NuGet.Protocol.Plugins;
using System.Security.Cryptography;
using GraduationProject.Data.Entities;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMYPharmacyService _pharmacyService;
        private readonly ApplicationDbContext _context;


        public PharmacyController(
           IAuthService authService,
           IMYPharmacyService pharmacyService,ApplicationDbContext context)
        {
            _authService = authService;
            _pharmacyService = pharmacyService;
            _context = context;
        }
        [HttpPost("Login")]

        public async Task<IActionResult> Login([FromBody] PharmacyLoginDTO FromReq)
        {
            if (string.IsNullOrEmpty(FromReq.Name) || string.IsNullOrEmpty(FromReq.Password))
            {
                return BadRequest(new { Message = "Username and password are required" });
            }

            // Step 2: Find the user in the database
            var pharmacy = await _context.pharmacies
                .FirstOrDefaultAsync(u => u.Email == FromReq.Email);

            if (pharmacy == null)
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            // Step 3: Verify the password (this example assumes you're storing hashed passwords)
            if (!VerifyPassword(FromReq.Password, pharmacy.HashPassword, pharmacy.saltPassword))
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }
            // 4. Hash the provided password with the pharmacy's salt
            string hashedInputPassword = HashPassword(FromReq.Password, pharmacy.saltPassword);

            if (hashedInputPassword != pharmacy.HashPassword)
            {
                return Unauthorized("Invalid username or password");
            }

            // Step 4: Determine user role (from your existing system)
            string role = pharmacy.role ?? "Pharmacy";

           
           

           // string Role = pharmacyRole?.role?.Name ?? "User"; // Default to "User" role if none found
           // string Role = pharmacy.role ?? "Pharmacy";
            // Step 5: Generate JWT token
            var token = _authService.GenerateToken(pharmacy, role);

            // Step 6: Return the token and user info
            return Ok(new PharamcyLoginResposeDTO
            {
                Token = token,
                PharmacyID = pharmacy.Id,
                PharmacyName = pharmacy.Name,
                Role = role
            });
        }

        private string HashPassword(string password, string salt)
        {
            using (var hmac = new HMACSHA512(Convert.FromBase64String(salt)))
            {
                var hashedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        private bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            // Convert the stored salt from string back to byte array
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            // Hash the input password with the same salt
            using (var hmac = new HMACSHA512(saltBytes))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                string computedHashString = Convert.ToBase64String(computedHash);

                // Compare the computed hash with the stored hash
                return computedHashString == storedHash;
            }
        }


        [HttpGet("profile{id}")]
        public async Task<IActionResult> GetProfile(int pharmacyId)
        {
           // var pharmacyId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
           
            var pharmacy = await _pharmacyService.GetProfileAsync(pharmacyId);

            if (pharmacy == null)
            {
                return NotFound("Pharmacy not found");
            }

            return Ok(pharmacy);
        }

        [HttpGet("orders{id}")]
        public async Task<IActionResult> GetOrdersby(int pharmacyId)
        {
           // var pharmacyId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var orders = await _pharmacyService.GetAllOrdersAsync(pharmacyId);

            return Ok(orders);
        }

        [HttpPut("UPprofile{id}")]
        public async Task<IActionResult> UpdateProfile(int pharmacyId, [FromBody] PharmacyLoginDTO fromRequest)
        {
           // var pharmacyId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _pharmacyService.UpdateProfileAsync(pharmacyId, fromRequest);

            if (result == null)
            {
                return NotFound("Pharmacy not found");
            }

            return Ok(result);
        }

    }
}
