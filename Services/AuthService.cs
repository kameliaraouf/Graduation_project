//using GraduationProject.Data;
//using GraduationProject.DTO;
//using GraduationProject.Data.Entities;
//using GraduationProject.Services.Interfaces;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Configuration;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Microsoft.AspNetCore.Http.HttpResults;
//using GraduationProject.Repositories.Intefaces;
//using GraduationProject.Repositories;

//namespace GraduationProject.Services
//{


//    public class AuthService : IAuthService
//    {


//           // private readonly ApplicationDbContext _context;
//            private readonly IConfiguration _configuration;
//        //private readonly IPharmacyRepo _mYPharmacyRepo;

//        public AuthService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public string GenerateToken(Pharmacy pharma, string role)
//        {
//            // Get the JWT key from configuration
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
//                _configuration["Jwt:Key"]));

//            // Create signing credentials
//            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            // Create claims for the token
//            var claims = new[]
//            {
//                new Claim(ClaimTypes.NameIdentifier, pharma.Id.ToString()),
//                new Claim(ClaimTypes.Name, pharma.Name),
//                new Claim(ClaimTypes.Role, role),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//            };

//            // Create the token with specified expiration
//            var token = new JwtSecurityToken(
//              //  issuer: _configuration["Jwt:Issuer"],
//               // audience: _configuration["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.Now.AddHours(8), // Token valid for 8 hours
//                signingCredentials: credentials
//            );

//            // Generate the token string
//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }

//}


// Add this to your AuthService.cs file
using GraduationProject.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GraduationProject.Services.Interfaces;

namespace GraduationProject.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Pharmacy pharmacy, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, pharmacy.Id.ToString()),
                new Claim(ClaimTypes.Name, pharmacy.Name),
                new Claim(ClaimTypes.Email, pharmacy.Email ?? ""),
                new Claim(ClaimTypes.Role, role)
            };

            return GenerateJwtToken(claims);
        }

        public string GenerateAdminToken(User admin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.UserID.ToString()),
                new Claim(ClaimTypes.Name, admin.UserName),
                // Make sure Role claim exists and is correctly formatted
                new Claim(ClaimTypes.Role, admin.Role)
            };

            return GenerateJwtToken(claims);
        }

        private string GenerateJwtToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenExpiry = _configuration.GetValue<int>("Jwt:TokenExpiryInHours");
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
               expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials
     

            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateUserToken(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Role, user.Role)
    };

            return GenerateJwtToken(claims); // Reuse the existing GenerateJwtToken method
        }

    }
}

