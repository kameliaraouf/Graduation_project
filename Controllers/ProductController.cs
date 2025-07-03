using GraduationProject.Data.DTO;
using GraduationProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using GraduationProject.Data.Entities;
using GraduationProject.Data.Models;
using GraduationProject.Data;
using Swashbuckle.AspNetCore.Annotations;



namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // 1. Create Product - Only for Pharmacy
        [HttpPost]
        [Authorize(Roles = "Pharmacy")]
        [SwaggerOperation(Summary = "Create a new product", Description = "Creates a new product for the authenticated pharmacy")]
        [SwaggerResponse(201, "The product was successfully created")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        public async Task<ActionResult<ProductResponseDTO>> CreateProduct([FromForm] CreateProductDTO createProductDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                string pharmacyName = User.FindFirst(ClaimTypes.Name)?.Value;

                var createdProduct = await _productService.CreateProductAsync(createProductDto, userId, pharmacyName);
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductID }, createdProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. Update Product - Only for Pharmacy who created it
        [HttpPut("{id}")]
        [Authorize(Roles = "Pharmacy")]
        [SwaggerOperation(Summary = "Update a product", Description = "Updates an existing product owned by the authenticated pharmacy")]
        [SwaggerResponse(200, "The product was successfully updated")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden - Not the owner of the product")]
        [SwaggerResponse(404, "Product not found")]
        public async Task<ActionResult<ProductResponseDTO>> UpdateProduct(int id, [FromForm] UpdateProductDTO updateProductDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var updatedProduct = await _productService.UpdateProductAsync(id, updateProductDto, userId);
                return Ok(updatedProduct);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 3. Delete Product - Only for Pharmacy who created it
        [HttpDelete("{id}")]
        [Authorize(Roles = "Pharmacy")]
        [SwaggerOperation(Summary = "Delete a product", Description = "Deletes an existing product owned by the authenticated pharmacy")]
        [SwaggerResponse(204, "The product was successfully deleted")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden - Not the owner of the product")]
        [SwaggerResponse(404, "Product not found")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                bool deleted = await _productService.DeleteProductAsync(id, userId);
                if (!deleted)
                    return NotFound(new { message = $"Product with ID {id} not found" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 4. Get All Products - Available for all
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get all products", Description = "Retrieves all products with their details and reviews")]
        [SwaggerResponse(200, "Success")]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 5. Get Product by ID - Available for all
        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Get product by ID", Description = "Retrieves a specific product by its ID with details and reviews")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(404, "Product not found")]
        public async Task<ActionResult<ProductResponseDTO>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                    return NotFound(new { message = $"Product with ID {id} not found" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // 6. Get Products by Name - Available for all
        [HttpGet("search/{name}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Search products by name", Description = "Searches for products containing the specified name")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Invalid search term")]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetProductsByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest(new { message = "Search term is required" });

                var products = await _productService.GetProductsByNameAsync(name);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("category/{category}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Search products by Category", Description = "Searches for products containing the specified Category")]
        [SwaggerResponse(200, "Success")]
        [SwaggerResponse(400, "Invalid search term")]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetProductsByCategory(string category)
        {
            try
            {
                // Log initial request info
                Console.WriteLine($"GetProductsByCategory: Received request for category '{category}'");

                // Debug override
                category = "SkincareProduct";
                Console.WriteLine($"GetProductsByCategory: Using hardcoded category '{category}'");

                if (string.IsNullOrWhiteSpace(category))
                    return BadRequest(new { message = "Search term is required" });

                var products = await _productService.GetProductsBycategoryAsync(category);

                // Log result count
                Console.WriteLine($"GetProductsByCategory: Found {products?.Count() ?? 0} products");

                // If empty, add more details
                if (products == null || !products.Any())
                {
                    Console.WriteLine("GetProductsByCategory: No products found, returning empty list");
                }
                else
                {
                    Console.WriteLine($"GetProductsByCategory: First product name: {products.First().Name}");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetProductsByCategory ERROR: {ex.Message}");
                Console.WriteLine($"GetProductsByCategory STACK: {ex.StackTrace}");
                return BadRequest(new { message = ex.Message });
            }
        }
        // 7. Add Product Review - Available for authenticated users
        [HttpPost("review")]
        [Authorize]
        [SwaggerOperation(Summary = "Add a product review", Description = "Adds a new review for a product by the authenticated user")]
        [SwaggerResponse(200, "Review added successfully")]
        [SwaggerResponse(400, "Invalid input or user already reviewed this product")]
        [SwaggerResponse(404, "Product not found")]
        public async Task<IActionResult> AddProductReview(ProductReviewDTO reviewDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                bool added = await _productService.AddProductReviewAsync(reviewDto, userId);

                if (!added)
                    return BadRequest(new { message = "Failed to add review" });

                return Ok(new { message = "Review added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message.Contains("not found")
                    ? NotFound(new { message = ex.Message })
                    : BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 8. Update Product Review - Available for authenticated users who wrote the review
        [HttpPut("review")]
        [Authorize]
        [SwaggerOperation(Summary = "Update a product review", Description = "Updates an existing review written by the authenticated user")]
        [SwaggerResponse(200, "Review updated successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden - Not the owner of the review")]
        [SwaggerResponse(404, "Review not found")]
        public async Task<IActionResult> UpdateProductReview(UpdateReviewDTO updateReviewDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                bool updated = await _productService.UpdateProductReviewAsync(updateReviewDto, userId);

                if (!updated)
                    return BadRequest(new { message = "Failed to update review or you are not authorized to update this review" });

                return Ok(new { message = "Review updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 9. Delete Product Review - Available for authenticated users who wrote the review
        [HttpDelete("review/by-product-name/{productName}")]
        [Authorize]
        [SwaggerOperation(Summary = "Delete a product review", Description = "Deletes a review written by the authenticated user")]
        [SwaggerResponse(200, "Review deleted successfully")]
        [SwaggerResponse(400, "Invalid input")]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden - Not the owner of the review")]
        [SwaggerResponse(404, "Review not found")]
        public async Task<IActionResult> DeleteProductReview(string productName)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                bool deleted = await _productService.DeleteProductReviewAsync(productName, userId);

                if (!deleted)
                    return BadRequest(new { message = "Failed to delete review or you are not authorized to delete this review" });

                return Ok(new { message = "Review deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
/*

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // 1. Create Product - Only for Pharmacy
        [HttpPost]
        [Authorize(Roles = "Pharmacy")]
        public async Task<ActionResult<ProductResponseDTO>> CreateProduct([FromForm] CreateProductDTO createProductDto)
        {

            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                string pharmacyName = User.FindFirst(ClaimTypes.Name)?.Value;

                var createdProduct = await _productService.CreateProductAsync(createProductDto, userId, pharmacyName);
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductID }, createdProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

         
        }

        // 2. Update Product - Only for Pharmacy who created it
        [HttpPut("{id}")]
        [Authorize(Roles = "Pharmacy")]
        public async Task<ActionResult<ProductResponseDTO>> UpdateProduct(int id, [FromForm] UpdateProductDTO updateProductDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var updatedProduct = await _productService.UpdateProductAsync(id, updateProductDto, userId);
                return Ok(updatedProduct);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 3. Delete Product - Only for Pharmacy who created it
        [HttpDelete("{id}")]
        [Authorize(Roles = "Pharmacy")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                bool deleted = await _productService.DeleteProductAsync(id, userId);
                if (!deleted)
                    return NotFound(new { message = $"Product with ID {id} not found" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 4. Get All Products - Available for all
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 5. Get Product by ID - Available for all
        [HttpGet("{id}")]
        [Authorize(Roles = "Pharmacy")]
        public async Task<ActionResult<ProductResponseDTO>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                    return NotFound(new { message = $"Product with ID {id} not found" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 6. Get Products by Name - Available for all
        [HttpGet("search/{name}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetProductsByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest(new { message = "Search term is required" });

                var products = await _productService.GetProductsByNameAsync(name);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 7. Add Product Review - Available for authenticated users
        [HttpPost("review")]
        [Authorize]
        public async Task<IActionResult> AddProductReview(ProductReviewDTO reviewDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                bool added = await _productService.AddProductReviewAsync(reviewDto, userId);

                if (!added)
                    return BadRequest(new { message = "Failed to add review" });

                return Ok(new { message = "Review added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



    }
}

*/


// Pharmacy specific endpoints
//[Authorize(Roles = "Pharmacy")]
//[HttpGet("pharmacy")]
//public async Task<ActionResult<IEnumerable<ProductDTO>>> GetPharmacyProducts()
//{
//    var pharmacyName = User.Identity.Name;  // Assuming pharmacy name is stored in the token
//    var products = await _productService.GetProductsByPharmacy(pharmacyName);
//    return Ok(products);
//}

//[Authorize(Roles = "Pharmacy")]
//[HttpGet("pharmacy/category/{category}")]
//public async Task<ActionResult<IEnumerable<ProductDTO>>> GetPharmacyProductsByCategory(Data.DTO.ProductCategory category)
//{
//    var pharmacyName = User.Identity.Name;
//    var products = await _productService.GetProductsByPharmacyAndCategory(pharmacyName, category);
//    return Ok(products);
//}

//[Authorize(Roles = "Pharmacy")]
//[HttpPost("pharmacy")]
//public async Task<ActionResult<ProductDTO>> CreatePharmacyProduct([FromBody] ProductDTO productDto)
//{
//    var pharmacyName = User.Identity.Name;

//    // Override any pharmacy name in the DTO with the authenticated pharmacy's name
//    productDto.PharmacyName = pharmacyName;

//    var product = await _productService.CreateProduct(productDto);
//    return CreatedAtAction(nameof(GetProduct), new { id = product.ProductID }, product);
//}

//[Authorize(Roles = "Pharmacy")]
//[HttpPut("pharmacy/{id}")]
//public async Task<IActionResult> UpdatePharmacyProduct(int id, [FromBody] ProductDTO productDto)
//{
//    var pharmacyName = User.Identity.Name;

//    var existingProduct = await _productService.GetProductById(id);
//    if (existingProduct == null)
//        return NotFound("Product not found");

//    // Verify the product belongs to this pharmacy
//    if (existingProduct.PharmacyName != pharmacyName)
//        return Forbid("You can only update your own products");

//    // Ensure the pharmacy can't change the pharmacy name
//    productDto.PharmacyName = pharmacyName;
//    productDto.ProductID = id;

//    await _productService.UpdateProduct(productDto);
//    return NoContent();
//}

//[Authorize(Roles = "Pharmacy")]
//[HttpDelete("pharmacy/{id}")]
//public async Task<IActionResult> DeletePharmacyProduct(int id)
//{
//    var pharmacyName = User.Identity.Name;

//    var existingProduct = await _productService.GetProductById(id);
//    if (existingProduct == null)
//        return NotFound("Product not found");

//    // Verify the product belongs to this pharmacy
//    if (existingProduct.PharmacyName != pharmacyName)
//        return Forbid("You can only delete your own products");

//    await _productService.DeleteProduct(id);
//    return NoContent();
//}

//[Authorize(Roles = "Admin")]
//[HttpGet("categories")]
//public ActionResult<IEnumerable<string>> GetCategories()
//{
//    var categories = Enum.GetNames(typeof(Data.DTO.ProductCategory));
//    return Ok(categories);
//}

//[Authorize(Roles = "Admin")]
//[HttpGet("pharmacies")]
//public async Task<ActionResult<IEnumerable<string>>> GetPharmacies()
//{
//    var pharmacyNames = await _pharmacyService.GetAllPharmacyNames();
//    return Ok(pharmacyNames);
//}


