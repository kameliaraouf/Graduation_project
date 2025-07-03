using GraduationProject.Data.Entities;
using GraduationProject.DTO;
using GraduationProject.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GraduationProject.Data;
using Microsoft.CodeAnalysis;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<CartDTO>> GetCart()
        {
            int userId = GetUserIdFromToken();

            // Get the user's cart or create one if it doesn't exist
            var cart = await GetOrCreateCart(userId);

            // Map cart to CartDTO
            var cartDto = new CartDTO
            {
                Id = cart.Id,
                userid = cart.UserID,
                TotalPrice = cart.TotalPrice,
                TotalItems = cart.TotalItems,
                Items = await _context.CartItems
    .Where(ci => ci.CartID == cart.Id)
    .Select(ci => new CartItemDTO
    {
        productId = ci.ProductID,  // Map from ProductID
        Quantity = ci.Quantity
    })
    .ToListAsync()
            };

            return cartDto;
        }

        // POST: api/Cart
        [HttpPost("add")]
        public async Task<ActionResult<CartDTO>> AddToCart(AddToCartDTO addToCartDto)
        {
            int userId = GetUserIdFromToken();

            // Get the product
            var product = await _context.products.FindAsync(addToCartDto.ProductId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            // Check if product is in stock
            if (product.Quantity < addToCartDto.Quantity)
            {
                return BadRequest("Product is out of stock or has insufficient quantity");
            }

            // Get or create cart
            var cart = await GetOrCreateCart(userId);

            // Check if product already exists in cart
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartID == cart.Id && ci.ProductID == addToCartDto.ProductId);

            if (existingCartItem != null)
            {
                // Update existing cart item
                int newQuantity = existingCartItem.Quantity + addToCartDto.Quantity;

                // Check if new quantity exceeds stock
                if (newQuantity > product.Quantity)
                {
                    return BadRequest("Cannot add more items than available in stock");
                }

                existingCartItem.Quantity = newQuantity;
            }
            else
            {
                // Add new cart item
                var cartItem = new CartItem
                {
                    CartID = cart.Id,
                    ProductID = addToCartDto.ProductId,
                    Quantity = addToCartDto.Quantity,
                    price = product.Price
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            // Update cart totals
            await UpdateCartTotals(cart.Id);

            await _context.SaveChangesAsync();

            return await GetCart();
        }

        // PUT: api/Cart/update
        [HttpPut("update")]
        public async Task<ActionResult<CartDTO>> UpdateCartItem(UpateCartItemDTO updateCartItemDto)
        {
            int userId = GetUserIdFromToken();

            // Get the cart item
            var cartItem = await _context.CartItems
                .Include(ci => ci.cart)
                .FirstOrDefaultAsync(ci => ci.ProductID == updateCartItemDto.productId);

            if (cartItem == null)
            {
                //Console.WriteLine("error 1");
                return NotFound("Cart item not found");
            }

            // Verify this cart belongs to the user
            if (cartItem.cart.UserID != userId)
            {
                return Forbid();
            }

            // Get the product to check stock
            var product = await _context.products.FindAsync(cartItem.ProductID);
            if (product == null)
            {

              //  Console.WriteLine("error 2");

                return NotFound("Product not found");
            }

            // Handle removal if quantity is 0
            if (updateCartItemDto.Quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                // Check if quantity is available in stock
                if (updateCartItemDto.Quantity > product.Quantity)
                {
                    return BadRequest("Cannot add more items than available in stock");
                }

                // Update quantity
                cartItem.Quantity = updateCartItemDto.Quantity;
            }
            await _context.SaveChangesAsync();

            // Update cart totals
            await UpdateCartTotals(cartItem.CartID);

            await _context.SaveChangesAsync();

            return await GetCart();
        }

        // DELETE: api/Cart/5
        [HttpDelete("{productId}")]
        public async Task<ActionResult<CartDTO>> RemoveCartItem(int productId)
        {
            int userId = GetUserIdFromToken();
            var cart = await GetOrCreateCart(userId);
            var cartItem = await _context.CartItems
        .FirstOrDefaultAsync(ci => ci.CartID == cart.Id && ci.ProductID == productId);

            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            // Verify this cart belongs to the user
            if (cartItem.cart.UserID != userId)
            {
                return Forbid();
            }

            _context.CartItems.Remove(cartItem);

            // Update cart totals
            await UpdateCartTotals(cartItem.CartID);

            await _context.SaveChangesAsync();

            return await GetCart();
        }

        // DELETE: api/Cart/clear
        [HttpDelete("clear")]
        public async Task<ActionResult<CartDTO>> ClearCart()
        {
            int userId = GetUserIdFromToken();

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null)
            {
                return NotFound("Cart not found");
            }

            // Remove all cart items
            _context.CartItems.RemoveRange(cart.CartItems);

            // Reset cart totals
            cart.TotalItems = 0;
            cart.TotalPrice = 0;

            await _context.SaveChangesAsync();

            return await GetCart();
        }

        // Helper methods
        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            return int.Parse(userIdClaim.Value);
        }

        private async Task<Cart> GetOrCreateCart(int userId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserID == userId);

            if (cart == null)
            {
                // Create new cart for user
                cart = new Cart
                {
                    UserID = userId,
                    TotalPrice = 0,
                    TotalItems = 0
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        private async Task UpdateCartTotals(int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart == null) return;

            var cartItems = await _context.CartItems
                .Where(ci => ci.CartID == cartId)
                .ToListAsync();

            cart.TotalItems = cartItems.Sum(ci => ci.Quantity);
            cart.TotalPrice = cartItems.Sum(ci => ci.price * ci.Quantity);

            _context.Carts.Update(cart);
        }
    }
}