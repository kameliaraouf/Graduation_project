using GraduationProject.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GraduationProject.Services.Interfaces
{
    public interface IProductService
    {

        Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO createProductDto, int userId, string pharmacyName);
        Task<ProductResponseDTO> UpdateProductAsync(int productId, UpdateProductDTO updateProductDto, int userId);
        Task<bool> DeleteProductAsync(int productId, int userId);
        Task<IEnumerable<ProductResponseDTO>> GetAllProductsAsync();
        Task<ProductResponseDTO> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductResponseDTO>> GetProductsByNameAsync(string name);
        Task<bool> AddProductReviewAsync(ProductReviewDTO reviewDto, int userId);
        Task<bool> UpdateProductReviewAsync(UpdateReviewDTO updateReviewDto, int userId);
        Task<bool> DeleteProductReviewAsync(string productName, int userId);
        Task<string> SaveImageAsync(IFormFile imageFile);
        Task<IEnumerable<ProductResponseDTO>> GetProductsBycategoryAsync(string category);







        // Define this in the interface
        // Task<IEnumerable<ProductDTO>> GetProductsByPharmacy(string pharmacyName);
        // Task<IEnumerable<ProductDTO>> GetProductsByPharmacyAndCategory(string pharmacyName, ProductCategoryDto category);


    }
}



