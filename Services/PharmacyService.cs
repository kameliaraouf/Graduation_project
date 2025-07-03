using GraduationProject.Data.DTO;
using GraduationProject.Data.Entities;
using GraduationProject.Repositories;
using GraduationProject.Repositories.Interfaces;
using GraduationProject.Services.Interfaces;

namespace GraduationProject.Services
{
    public class PharmacyService : IPharmacyService
    {
       

            private readonly IPharmacyRepository _pharmacyRepository;

            public PharmacyService(IPharmacyRepository pharmacyRepository)
            {
                _pharmacyRepository = pharmacyRepository;
            }

            public async Task<bool> PharmacyExistsByName(string name)
            {
                return await _pharmacyRepository.ExistsByNameAsync(name);
            }

            public async Task<IEnumerable<string>> GetAllPharmacyNames()
            {
                return await _pharmacyRepository.GetAllPharmacyNamesAsync();
            }
///
            public async Task<string> RegisterPharmacyAsync(PharmacyRegisterDTO model)
            {
                // Check if pharmacy with the same name already exists
                var pharmacyExists = await _pharmacyRepository.ExistsByNameAsync(model.Name);
                if (pharmacyExists)
                {
                    return "Pharmacy with this name already exists";
                }

                // Check if pharmacy with the same email already exists
                var emailExists = await _pharmacyRepository.ExistsByEmailAsync(model.Email);
                if (emailExists)
                {
                    return "Pharmacy with this email already exists";
                }

                // Hash the password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            //added by kamelia****************

            string passwordSalt = GenerateSalt(); // Create a method to generate a random salt
               string passwordHash2 = HashPassword(model.Password, passwordSalt); // Create a hashing method

                // Create new pharmacy entity
                var pharmacy = new Pharmacy
                {
                    Email = model.Email,
                    Password = passwordHash,
                    //added by kamelia****************
                   HashPassword  = passwordHash2, // Use the hashed password
                   saltPassword = passwordSalt,
                    Name = model.Name,
                    Phone = model.Phone,
                    Address = model.Address,
                    role= "Pharmacy"

                };

                // Save the pharmacy
                await _pharmacyRepository.AddAsync(pharmacy);

                return "Pharmacy registered successfully";
            }

        //added by kamelia****************

        private string GenerateSalt()
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[16]; // 16 bytes = 128 bits
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        private string HashPassword(string password, string salt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(salt)))
            {
                var hashedBytes = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }





        public async Task<Pharmacy> GetPharmacyByIdAsync(int id)
        {
            return await _pharmacyRepository.GetPharmacyByIdAsync(id);
        }

        public async Task<bool> DeletePharmacyAsync(int pharmacyId)
        {
            // Check if pharmacy exists
            var pharmacy = await _pharmacyRepository.GetPharmacyByIdAsync(pharmacyId);
            if (pharmacy == null)
            {
                return false; // If pharmacy not found, return false
            }

            // Remove the pharmacy from the repository
            _pharmacyRepository.Remove(pharmacy);
            return true;
        }
    }     
    }
