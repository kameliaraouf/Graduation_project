using GraduationProject.Data.Entities;
using GraduationProject.Data;
using GraduationProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using GraduationProject.Data.Models;
using Microsoft.CodeAnalysis;



namespace GraduationProject.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByNameAndPharmacyAsync(string productName, string pharmacyName)
        {
            return await _context.products
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Name == productName && p.pharmacyName == pharmacyName);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _context.products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }
         /*  public async Task<Product> GetProductByIdAsync(int productId)
          {
              return await _context.products
                  .Include(p => p.Reviews)
                      .ThenInclude(r => r.User)
                  .Include(p => p.user)
                  .FirstOrDefaultAsync(p => p.ProductID == productId);
          }
        */
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.products
        .AsNoTracking() // Add this to prevent tracking issues
        .Include(p => p.Reviews)
            .ThenInclude(r => r.User)
        .Include(p => p.user)
        .FirstOrDefaultAsync(p => p.ProductID == productId);
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.products
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .Include(p => p.user)
                .ToListAsync();
        }

        public async Task<bool> AddProductReviewAsync(ProductReview review)
        {
            // Check if a review from this user for this product already exists
            var existingReview = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.ProductID == review.ProductID && r.UserID == review.UserID);

            if (existingReview != null)
            {
                return false; // Review already exists, should use the update endpoint instead
            }
            else
            {
                // Add new review
                await _context.ProductReviews.AddAsync(review);
            }

            await _context.SaveChangesAsync();

            // Update the product's average rating
            await UpdateProductAverageRatingAsync(review.ProductID);

            return true;
        }

        public async Task<bool> UpdateProductReviewAsync(int reviewId, double rating, string reviewText, int userId)
        {
            var review = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.ReviewID == reviewId);

            if (review == null)
                return false;

            // Check if the user is the owner of the review
            if (review.UserID != userId)
                return false;

            // Update review
            review.Rating = rating;
            review.ReviewText = reviewText;
            review.ReviewDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Update the product's average rating
            await UpdateProductAverageRatingAsync(review.ProductID);

            return true;
        }

     
        public async Task<bool> DeleteProductReviewAsync(ProductReview review)
        {
            if (review == null)
                return false;

            // Get the product ID before removing the review
            var productId = review.ProductID;

            // Remove the review
            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();

            // Update the product's average rating
            await UpdateProductAverageRatingAsync(productId);

            return true;
        }


        public async Task<ProductReview> GetReviewByIdAsync(int reviewId)
        {
            return await _context.ProductReviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReviewID == reviewId);
        }

        public async Task<ProductReview> GetUserReviewForProductAsync(int productId, int userId)
        {
            return await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.ProductID == productId && r.UserID == userId);
        }

        private async Task UpdateProductAverageRatingAsync(int productId)
        {
            var product = await _context.products.FindAsync(productId);
            if (product != null)
            {
                // Calculate average rating
                var reviews = await _context.ProductReviews
                    .Where(r => r.ProductID == productId)
                    .ToListAsync();

                if (reviews.Any())
                {
                    product.AverageRating = reviews.Average(r => r.Rating);
                }
                else
                {
                    product.AverageRating = 0;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            return await _context.products
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .Include(p => p.user)
                .Where(p => p.Name.Contains(name))
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            Console.WriteLine($"GetProductsByCategoryAsync: Starting with category '{category}'");

            // First check if there are any products at all
            var totalProductCount = await _context.products.CountAsync();
            Console.WriteLine($"GetProductsByCategoryAsync: Total products in database: {totalProductCount}");

            // Check if Category enum exists
            var validEnum = Enum.TryParse<ProductCategory>(category, true, out var categoryEnum);
            Console.WriteLine($"GetProductsByCategoryAsync: Enum parse success: {validEnum}, value: {categoryEnum}");

            IQueryable<Product> query;
            if (validEnum)
            {
                Console.WriteLine($"GetProductsByCategoryAsync: Using enum comparison with {categoryEnum}");
                query = _context.products
                    .Include(p => p.Reviews)
                        .ThenInclude(r => r.User)
                    .Include(p => p.user)
                    .Where(p => p.Category == categoryEnum);
            }
            else
            {
                Console.WriteLine($"GetProductsByCategoryAsync: Using string comparison with '{category}'");
                query = _context.products
                    .Include(p => p.Reviews)
                        .ThenInclude(r => r.User)
                    .Include(p => p.user)
                    .Where(p => p.Category.ToString().Contains(category, StringComparison.OrdinalIgnoreCase));
            }

            // Get the SQL (if possible)
            try
            {
                var sql = query.ToQueryString(); // Only works with EF Core 3.0+
                Console.WriteLine($"GetProductsByCategoryAsync: Generated SQL: {sql}");
            }
            catch
            {
                Console.WriteLine("GetProductsByCategoryAsync: Could not get SQL query string");
            }

            var result = await query.ToListAsync();

            Console.WriteLine($"GetProductsByCategoryAsync: Query returned {result.Count} products");

            // If empty but we know we have products, log some sample categories
            if (result.Count == 0 && totalProductCount > 0)
            {
                var sampleCategories = await _context.products
                    .Take(5)
                    .Select(p => new { p.ProductID, p.Name, Category = p.Category.ToString() })
                    .ToListAsync();

                Console.WriteLine("GetProductsByCategoryAsync: Sample products in database:");
                foreach (var sample in sampleCategories)
                {
                    Console.WriteLine($"  - ID: {sample.ProductID}, Name: {sample.Name}, Category: {sample.Category}");
                }
            }

            return result;
        }
        public async Task<Product> GetProductByNameAsync(string name)
        {
            return await _context.products
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .Include(p => p.user)
                .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.products.FindAsync(productId);

            if (product == null)
                return false;

            // First remove all related reviews
            var reviews = await _context.ProductReviews
                .Where(r => r.ProductID == productId)
                .ToListAsync();

            _context.ProductReviews.RemoveRange(reviews);

            // Then remove the product
            _context.products.Remove(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ProductExistsAsync(int productId)
        {
            return await _context.products.AnyAsync(p => p.ProductID == productId);
        }

        public async Task<bool> IsProductOwnerAsync(int productId, int pharmacyUserId)
        {
            var product = await _context.products.FindAsync(productId);
            return product != null && product.UserID == pharmacyUserId;
        }
    }
}
/*

namespace GraduationProject.Data.Repositories
{

    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByNameAndPharmacyAsync(string productName, string pharmacyName)
        {
            return await _context.products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Name == productName && p.pharmacyName == pharmacyName);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _context.products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.products
                .Include(p => p.Reviews)
                .Include(p => p.Reviews)
                .Include(p => p.user)
                .FirstOrDefaultAsync(p => p.ProductID == productId);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.products

                .Include(p => p.Reviews)
                .Include(p => p.user)
                .ToListAsync();
        }

        public async Task<bool> AddProductReviewAsync(ProductReview review)
        {
            // Check if a review from this user for this product already exists
            var existingReview = await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.ProductID == review.ProductID && r.UserID == review.UserID);

            if (existingReview != null)
            {
                // Update existing review
                existingReview.Rating = review.Rating;
                existingReview.ReviewText = review.ReviewText;
                existingReview.ReviewDate = DateTime.UtcNow;
            }
            else
            {
                // Add new review
                await _context.ProductReviews.AddAsync(review);
            }

            await _context.SaveChangesAsync();

            // Update the product's average rating and review list
            var product = await _context.products.FindAsync(review.ProductID);
            if (product != null)
            {
                // Calculate average rating
                var averageRating = await _context.ProductReviews
                    .Where(r => r.ProductID == review.ProductID)
                    .AverageAsync(r => r.Rating);

                product.AverageRating = averageRating;

                // Get all reviews for this product
                var allReviews = await _context.ProductReviews
                    .Where(r => r.ProductID == review.ProductID)
                    .OrderByDescending(r => r.ReviewDate)
                    .Select(r => r.ReviewText)
                    .ToListAsync();

                // Convert the list of reviews to a string with a separator
                product.Review = string.Join(" | ", allReviews);

                await _context.SaveChangesAsync();
            }

            return true;
        }
        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            return await _context.products
                .Include(p => p.Reviews)
                .Include(p => p.user)
                .Where(p => p.Name.Contains(name))
                .ToListAsync();
        }
        public async Task<Product> GetProductByNameAsync(string name)
        {
            return await _context.products
                .Include(p => p.Reviews)
                .Include(p => p.user)
                .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
        }
       

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.products.FindAsync(productId);

            if (product == null)
                return false;

            _context.products.Remove(product);
            return await _context.SaveChangesAsync() > 0;
        }
       
        public async Task<bool> ProductExistsAsync(int productId)
        {
            return await _context.products.AnyAsync(p => p.ProductID == productId);
        }

        public async Task<bool> IsProductOwnerAsync(int productId, int pharmacyUserId)
        {
            var product = await _context.products.FindAsync(productId);
            return product != null && product.UserID == pharmacyUserId;
        }
    }
    }

*/