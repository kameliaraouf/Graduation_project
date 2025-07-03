using GraduationProject.Data.DTO;
using GraduationProject.Services;
using GraduationProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Security.Claims;

namespace GraduationProject.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPharmacyService _pharmacyService;
        private readonly IAuthService _authService;
        public UserController(IUserService userService, IPharmacyService pharmacyService, IAuthService authService)
        {
            _userService = userService;
            _pharmacyService = pharmacyService;
            _authService = authService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUserAsync(model);
            if (result == "User registered successfully")
                return Ok(new { Message = result });
            else
                return BadRequest(new { Message = result });
        }






        [HttpPost("User_login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDTO)
        {
            if (loginDTO == null || string.IsNullOrEmpty(loginDTO.UserName))
                return BadRequest("Username is required.");

            if (string.IsNullOrEmpty(loginDTO.Password) &&
                string.IsNullOrEmpty(loginDTO.Email) &&
                string.IsNullOrEmpty(loginDTO.phone))
            {
                return BadRequest("You must provide either password, email, or phone with username.");
            }

            var result = await _userService.LoginUserAsync(loginDTO);

            if (result == null)
                return Unauthorized("Empty Fields");

            return Ok(new { Token = result });
        }






        [HttpPost("Admin_login")]
        public async Task<IActionResult> adminLogin([FromBody] AdminDTO adminDTO)
        {
            if (string.IsNullOrEmpty(adminDTO.UserName) && string.IsNullOrEmpty(adminDTO.Password))
            {
                return BadRequest("Fields are Empty");
            }
            else if (string.IsNullOrEmpty(adminDTO.UserName))
            {
                return BadRequest("UserName is Empty");
            }


            else if (string.IsNullOrEmpty(adminDTO.Password))
            {
                return BadRequest("Password is Empty");
            }

            var result = await _userService.LoginAdminAsync(adminDTO);
            if (result == null)
                return Unauthorized("Invalid username or password");

            return Ok(new { Token = result });
            //  return Ok("Admin Login Successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-pharmacy")]
        public async Task<IActionResult> RegisterPharmacy([FromBody] PharmacyRegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _pharmacyService.RegisterPharmacyAsync(model);

            if (result == "Pharmacy registered successfully")
                return Ok(new { Message = result });
            else
                return BadRequest(new { Message = result });
        }
        // Admin delete pharmacy account
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-pharmacy/{id}")]
        public async Task<IActionResult> DeletePharmacy(int id)
        {
            var deleteResult = await _pharmacyService.DeletePharmacyAsync(id);
            if (!deleteResult)
                return NotFound("Pharmacy not found");

            return Ok(new { Message = "Pharmacy deleted successfully" });
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserEditDTO model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Ensure you're fetching real user data
            var updatedUser = await _userService.UpdateUserAsync(userId, model);

            if (updatedUser == null)
                return BadRequest(new { Message = "User update failed" });

            // Returning the actual updated user data, not just a success message
            return Ok(new { Message = "User updated successfully", User = updatedUser });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.Phone,
                user.Email
            });
        }


      /*  [Authorize]
          [HttpDelete("delete-account")]
           public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDTO model)
           {
               var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
               var user = await _userService.GetUserByIdAsync(userId);

               if (user == null)
                   return NotFound("User not found");

               if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                   return Unauthorized("Incorrect password");

               var deleteResult = await _userService.DeleteUserAsync(userId);

               if (!deleteResult)
                   return BadRequest("Failed to delete user");

               return Ok(new { Message = "Account deleted successfully" });
           }
        */


    } }

      