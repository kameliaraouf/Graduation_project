using Azure.Messaging;
using GraduationProject.DTO;

using GraduationProject.Repositories.Intefaces;
using GraduationProject.Services.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Core;
using GraduationProject.Data;

using Microsoft.AspNetCore.Http;


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Data.Entities;

namespace GraduationProject.Services
{
    public class MyPharmacyService : IMYPharmacyService
    {

        private readonly IPharmacyRepo _pharmacyRepository;

        public MyPharmacyService(IPharmacyRepo pharmacyRepository)
        {
            _pharmacyRepository = pharmacyRepository;
        }
        public async Task<Pharmacy> GetProfileAsync(int pharmacyId)
        {
            return await _pharmacyRepository.GetPharmacyByIdAsync(pharmacyId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(int pharmacyId)
        {
            return await _pharmacyRepository.GetAllOrdersByPharmacyIdAsync(pharmacyId);
        }


        public async Task<string> UpdateProfileAsync(int pharmacyId, PharmacyLoginDTO profileDto)
        {
            var pharmacy = await _pharmacyRepository.GetPharmacyByIdAsync(pharmacyId);

            if (pharmacy == null)
            {
                return null;
            }

            // Update pharmacy properties
            pharmacy.Password = profileDto.Password;
            pharmacy.Email = profileDto.Email;
            pharmacy.Name = profileDto.Name;
            pharmacy.Address = profileDto.Address;
            pharmacy.Phone = profileDto.Phone;

            await _pharmacyRepository.SaveChangesAsync();

            return "Profile updated successfully";
        }
    }
}
