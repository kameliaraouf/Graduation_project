using GraduationProject.Data.DTO;
using GraduationProject.Data.Entities;
using GraduationProject.Data.Models;
using GraduationProject.Data.Repositories;
using GraduationProject.Repositories;
using GraduationProject.Repositories.Interfaces;
using GraduationProject.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;


namespace GraduationProject.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductService(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO createProductDto, int userId, string pharmacyName)
        {
            var existingProduct = await _productRepository.GetProductByNameAndPharmacyAsync(createProductDto.Name, pharmacyName);
            if (existingProduct != null)
            {
                throw new InvalidOperationException("Product with this name already exists for this pharmacy.");
            }

            // Save the image and get the path
            string imagePath = await SaveImageAsync(createProductDto.ImageFile);

            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Img = imagePath,
                Price = createProductDto.Price,
                Quantity = createProductDto.Quantity,
                Category = createProductDto.Category,
                UserID = userId,
                pharmacyName = pharmacyName,
                AverageRating = 0,
                Reviews = new List<ProductReview>()
            };

            var createdProduct = await _productRepository.CreateProductAsync(product);
            return MapToProductResponseDto(createdProduct);
        }

        public async Task<ProductResponseDTO> UpdateProductAsync(int productId, UpdateProductDTO updateProductDto, int userId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {productId} not found");

            if (product.UserID != userId)
                throw new UnauthorizedAccessException("You are not authorized to update this product");

            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.Price = updateProductDto.Price;
            product.Quantity = updateProductDto.Quantity;
            product.Category = updateProductDto.Category;

            // Update image only if a new one is provided
            if (updateProductDto.ImageFile != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(product.Img))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.Img);
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                // Save new image
                product.Img = await SaveImageAsync(updateProductDto.ImageFile);
            }

            var updatedProduct = await _productRepository.UpdateProductAsync(product);
            return MapToProductResponseDto(updatedProduct);
        }

        public async Task<bool> DeleteProductAsync(int productId, int userId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                return false;

            if (product.UserID != userId)
                throw new UnauthorizedAccessException("You are not authorized to delete this product");

            // Delete the image file
            if (!string.IsNullOrEmpty(product.Img))
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.Img);
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }

            return await _productRepository.DeleteProductAsync(productId);
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return products.Select(MapToProductResponseDto);
        }

        public async Task<ProductResponseDTO> GetProductByIdAsync(int productId)
        {
            
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                return null;

            return MapToProductResponseDto(product);
        }
    

        public async Task<IEnumerable<ProductResponseDTO>> GetProductsByNameAsync(string name)
        {
            var products = await _productRepository.GetProductsByNameAsync(name);
            return products.Select(MapToProductResponseDto);
        }

        public async Task<bool> AddProductReviewAsync(ProductReviewDTO reviewDto, int userId)
        {
            // Find the product by name
            var products = await _productRepository.GetProductsByNameAsync(reviewDto.ProductName);
            var product = products.FirstOrDefault();

            if (product == null)
                throw new InvalidOperationException($"Product with name '{reviewDto.ProductName}' not found");

            // Check if the user already has a review for this product
            var existingReview = await _productRepository.GetUserReviewForProductAsync(product.ProductID, userId);
            if (existingReview != null)
                throw new InvalidOperationException("You already have a review for this product. Please use the update endpoint instead.");

            // Create review object
            var review = new ProductReview
            {
                ProductID = product.ProductID,
                UserID = userId,
                Rating = reviewDto.Rating,
                ReviewText = reviewDto.ReviewText,
                ReviewDate = DateTime.UtcNow
            };

            return await _productRepository.AddProductReviewAsync(review);
        }

        public async Task<bool> UpdateProductReviewAsync(UpdateReviewDTO updateReviewDto, int userId)
        {
            // Find the product by name
            var products = await _productRepository.GetProductsByNameAsync(updateReviewDto.ProductName);
            var product = products.FirstOrDefault();

            if (product == null)
                throw new InvalidOperationException($"Product with name '{updateReviewDto.ProductName}' not found");

            // Find the user's review for this product
            var review = await _productRepository.GetUserReviewForProductAsync(product.ProductID, userId);
            if (review == null)
                throw new InvalidOperationException("You don't have a review for this product yet. Please create a review first.");

            // Update the review
            return await _productRepository.UpdateProductReviewAsync(
                review.ReviewID,
                updateReviewDto.Rating,
                updateReviewDto.ReviewText,
                userId);
        }

        public async Task<bool> DeleteProductReviewAsync(string productName, int userId)
        {
            // Find the product by name
            var products = await _productRepository.GetProductsByNameAsync(productName);
            var product = products.FirstOrDefault();

            if (product == null)
                throw new InvalidOperationException($"Product with name '{productName}' not found");

            // Find the user's review for this product
            var review = await _productRepository.GetUserReviewForProductAsync(product.ProductID, userId);
            if (review == null)
                throw new InvalidOperationException("You don't have a review for this product.");

            // Pass review directly to delete method
            return await _productRepository.DeleteProductReviewAsync(review);
        }







        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image file is required");

            try
            {
                // Log the root path
                Console.WriteLine($"WebRootPath: {_webHostEnvironment.WebRootPath}");

                // Combine path to target folder
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                Console.WriteLine($"Uploads Folder: {uploadsFolder}");

                // Ensure directory exists
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                Console.WriteLine($"Saving file to: {filePath}");

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Return relative path for database storage
                return Path.Combine("images", "products", uniqueFileName).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
                throw; // rethrow for higher-level handling
            }
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetProductsBycategoryAsync(string category)
        {
            Console.WriteLine($"GetProductsBycategoryAsync: Starting with category '{category}'");

            var products = await _productRepository.GetProductsByCategoryAsync(category);

            Console.WriteLine($"GetProductsBycategoryAsync: Repository returned {products?.Count() ?? 0} products");

            if (products == null || !products.Any())
            {
                Console.WriteLine("GetProductsBycategoryAsync: No products from repository");
                return new List<ProductResponseDTO>();
            }

            var dtos = products.Select(MapToProductResponseDto).ToList();

            Console.WriteLine($"GetProductsBycategoryAsync: Mapped to {dtos.Count} DTOs");

            return dtos;
        }
        private ProductResponseDTO MapToProductResponseDto(Product product)
                {
                    var reviewDtos = new List<ReviewDetailDTO>();

                    if (product.Reviews != null && product.Reviews.Any())
                    {
                        reviewDtos = product.Reviews.Select(r => new ReviewDetailDTO
                        {
                            Username = r.User?.UserName ?? "Unknown User",
                            Rating = r.Rating,
                            ReviewText = r.ReviewText,
                            ReviewDate = r.ReviewDate
                        }).ToList();
                    }
            
                    return new ProductResponseDTO
                    {
                        ProductID = product.ProductID,
                        Name = product.Name,
                        Description = product.Description,
                        Img = product.Img,
                        Price = product.Price,
                        Quantity = product.Quantity,
                        Category = product.Category.ToString(),
                        PharmacyName = product.pharmacyName,
                        AverageRating = product.AverageRating ?? 0,
                       // Review = string.Empty, // This is now deprecated, using Reviews list instead
                        Reviews = reviewDtos
                    };
                }
       
       

    }
}

