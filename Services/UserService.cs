using GraduationProject.Data.DTO;
using GraduationProject.Data.Entities;
using GraduationProject.Data;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraduationProject.Repositories.Interfaces;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation.AspNetCore;
using System.ComponentModel.DataAnnotations;
using GraduationProject.Repositories;
using System.Data;
//using GraduationProject.Migrations;

namespace GraduationProject.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public UserService(IUserRepository userRepository, IConfiguration configuration,IAuthService authService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _authService = authService;
        }
  
        public async Task<string> RegisterUserAsync(UserRegisterDTO registerDTO)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(registerDTO);

            if (!Validator.TryValidateObject(registerDTO, validationContext, validationResults, true))
            {
                return string.Join("; ", validationResults.Select(v => v.ErrorMessage));
            }

            if (registerDTO.UserName.ToLower() == "admin")
            {
                return "Admin registration is not allowed!";
            }
            var existingUser = await _userRepository.GetByUsernameAsync(registerDTO.UserName);
            if (existingUser != null)
                return "User already exists";

            var user = new User
            { 
                Password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password),

                UserName=registerDTO.UserName,
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Phone = registerDTO.Phone,
                Email = registerDTO.Email,
                Role = "User"
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return "User registered successfully";
        }


        //public async Task<string> LoginAdminAsync(AdminDTO adminDTO)
        //{

        //    if (adminDTO.UserName?.ToLower() == "admin")
        //    {
        //        var admin = await _userRepository.GetByUsernameAsync("admin");
        //        if (admin == null)
        //            return null; // Admin not found 

        //        if (!BCrypt.Net.BCrypt.Verify(adminDTO.Password, admin.Password))
        //            return null; // Password incorrect

        //        return GenerateJwtToken(admin);
        //    }
        //    else return null;
        //}
        public async Task<string> LoginAdminAsync(AdminDTO adminDTO)
        {
            var admin = await _userRepository.GetByUsernameAsync("admin");

            if (admin == null)
                return null;

            // Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(adminDTO.Password, admin.Password);
            if (!isPasswordValid)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(adminDTO.Password, admin.Password))
               return null; // Password incorrect
            // Use the updated AuthService to generate Admin token
            return _authService.GenerateAdminToken(admin);
        }

   
      
        public async Task<string> LoginUserAsync(UserLoginDTO loginDTO)
        {
            if (string.IsNullOrEmpty(loginDTO.UserName))
                return null;

            var user = await _userRepository.FirstOrDefaultAsync(u => u.UserName == loginDTO.UserName);

            if (user == null)
                return null;

            bool isPasswordMatch = !string.IsNullOrEmpty(loginDTO.Password) &&
                                   BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password);

            bool isEmailMatch = !string.IsNullOrEmpty(loginDTO.Email) &&
                                loginDTO.Email == user.Email;

            bool isPhoneMatch = !string.IsNullOrEmpty(loginDTO.phone) &&
                                loginDTO.phone == user.Phone;

            if (isPasswordMatch || isEmailMatch || isPhoneMatch)
            {
                return _authService.GenerateUserToken(user);
            }

            return null; // None of the combinations matched
        }






        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<UserDTO> UpdateUserAsync(int userId, UserEditDTO editDTO)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            // Validate current password before allowing an update (if provided)
            if (!string.IsNullOrEmpty(editDTO.Password))
            {
                if (!BCrypt.Net.BCrypt.Verify(editDTO.Password, user.Password))
                {
                    throw new UnauthorizedAccessException("Incorrect current password");
                }
            }

            // Update only fields that are not null or empty (or whitespace)
            if (!string.IsNullOrWhiteSpace(editDTO.FirstName))
                user.FirstName = editDTO.FirstName;

            if (!string.IsNullOrWhiteSpace(editDTO.LastName))
                user.LastName = editDTO.LastName;

            if (!string.IsNullOrWhiteSpace(editDTO.Phone))
                user.Phone = editDTO.Phone;

            if (!string.IsNullOrWhiteSpace(editDTO.Email))
                user.Email = editDTO.Email;

            // Update password only if a new password is provided and it's different from the current password
            if (!string.IsNullOrEmpty(editDTO.NewPassword))
            {
                // Check the current password if provided
                if (string.IsNullOrEmpty(editDTO.Password) || !BCrypt.Net.BCrypt.Verify(editDTO.Password, user.Password))
                {
                    throw new UnauthorizedAccessException("Incorrect current password");
                }

                // If a new password is provided, hash it and update the password
                user.Password = BCrypt.Net.BCrypt.HashPassword(editDTO.NewPassword);
            }

            // Save changes to the database
            bool isUpdated = await _userRepository.SaveChangesAsync();
            if (!isUpdated) return null;

            // Return the updated user details as a DTO
            return new UserDTO
            {
                UserID = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone
            };
        }


       





        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            _userRepository.Delete(user);
            return await _userRepository.SaveChangesAsync();
        }

    //    private string GenerateJwtToken(User user)
    //    {
    //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    //        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //        var claims = new List<Claim>
    //{
    //    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
    //    new Claim(ClaimTypes.Name, user.UserName),
    //    new Claim(ClaimTypes.Role, user.Role)
        
    //};

    //        var token = new JwtSecurityToken(
    //            _configuration["Jwt:Issuer"],
    //            _configuration["Jwt:Audience"],
    //            claims,
    //            expires: DateTime.UtcNow.AddHours(2),
    //            signingCredentials: credentials
    //        );

    //        return new JwtSecurityTokenHandler().WriteToken(token);
    //    }


    }
}
