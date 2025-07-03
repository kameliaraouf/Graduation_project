using GraduationProject.Data.Entities;
using GraduationProject.Data.Models;
using Newtonsoft.Json;

namespace GraduationProject.Repositories.Interfaces
{
     public interface IProductRepository
      {
          Task<Product> CreateProductAsync(Product product);
          Task<Product> GetProductByIdAsync(int productId);
          Task<IEnumerable<Product>> GetAllProductsAsync();
          Task<IEnumerable<Product>> GetProductsByNameAsync(string name);
          Task<Product> UpdateProductAsync(Product product);
          Task<bool> DeleteProductAsync(int productId);
         // Task<bool> AddProductReviewAsync(ProductReview review);
          Task<bool> ProductExistsAsync(int productId);
          Task<bool> IsProductOwnerAsync(int productId, int pharmacyUserId);
          Task<Product> GetProductByNameAndPharmacyAsync(string productName, string pharmacyName);
          Task<Product> GetProductByNameAsync(string name);

        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
          Task<bool> AddProductReviewAsync(ProductReview review);
          Task<bool> UpdateProductReviewAsync(int reviewId, double rating, string reviewText, int userId);
          Task<bool> DeleteProductReviewAsync(ProductReview review);
          Task<ProductReview> GetReviewByIdAsync(int reviewId);
          Task<ProductReview> GetUserReviewForProductAsync(int productId, int userId);
        
        



    }
  }
    

   
