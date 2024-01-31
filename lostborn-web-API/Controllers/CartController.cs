using lostborn_backend.Helpers;
using lostborn_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lostborn_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly DataContext _context;

        public CartsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Carts>>> GetCarts()
        {
            return await _context.Carts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Carts>> GetCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        [HttpPost]
        public async Task<ActionResult<Carts>> CreateCart(Carts cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCart), new { id = cart.ID }, cart);
        }
        private bool CartExists(int id)
        {
            return _context.Carts.Any(c => c.ID == id);
        }
        //API Call to update member using Id...(UPDATE)
        [HttpPut]
        public async Task<IActionResult> UpdateCart(Carts member)
        {
            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // if (!MemberExists(id)) { return NotFound(); }
                if (!CartExists(member.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
                throw;
            }

            // Fetch the updated data from the database
            var updatedMember = await _context.products.FindAsync(member.ID);

            // You can create a custom response object or use an anonymous object
            var response = new
            {
                Message = "User Cart Updated successfully",
                UpdatedData = updatedMember
            };

            // Return the custom response
            return Ok(response);
        }

        [HttpGet("{id}/cartitems")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems(int id)
        {
            var cartItems = await _context.CartItems.Where(ci => ci.CartID == id).ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return NotFound();
            }

            return cartItems;
        }

        [HttpPost("{id}/cartitems")]
        public async Task<ActionResult<CartItem>> AddCartItem(int id, CartItem cartItem)
        {
            var cart = await _context.Carts.FindAsync(id);

            if (cart == null)
            {
                return NotFound();
            }

            cartItem.CartID = id;
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCartItems), new { id }, cartItem);
        }
        [HttpDelete("{username}/cartitems/{itemId}")]
        public async Task<IActionResult> DeleteCartItem(string username, int itemId)
        {
            // Fetch the user based on the username
            var user = await _context.Users
                .Include(u => u.Cart)
                .ThenInclude(c => c.CartItems)
                .FirstOrDefaultAsync(u => u.username == username);

            if (user == null)
            {
                return NotFound($"User with username {username} not found");
            }

            // Fetch the cart item to delete
            var cartItemToDelete = user.Cart.CartItems.FirstOrDefault(ci => ci.ID == itemId);

            if (cartItemToDelete == null)
            {
                return NotFound($"Cart item with ID {itemId} not found in the user's cart");
            }

            // Remove the cart item from the collection
            user.Cart.CartItems.Remove(cartItemToDelete);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the updated user cart
            var updatedUserCart = await _context.Users
                .Include(u => u.Cart)
                .ThenInclude(c => c.CartItems)
                .FirstOrDefaultAsync(u => u.username == username);

            return Ok(updatedUserCart);
        }



        [HttpPost("{username}/cart/AddToCart")]
        public async Task<IActionResult> AddToCart(string username, [FromBody] CartItemRequest addToCartPayload)
        {
            // Validate input as needed

            // Fetch the user based on the username
            var user = await _context.Users
                .Include(u => u.Cart)
                .ThenInclude(c => c.CartItems)
                .FirstOrDefaultAsync(u => u.username == username);

            if (user == null)
            {
                // Handle the case where the user is not found
                return NotFound($"User with username {username} not found");
            }

            // Fetch the product based on the provided ProductID
            var product = await _context.products.FindAsync(addToCartPayload.ProductID);

            if (product == null)
            {
                // Handle the case where the product is not found
                return NotFound($"Product with ID {addToCartPayload.ProductID} not found");
            }

            // Check if the same product already exists in the user's cart
            var existingCartItem = user.Cart.CartItems.FirstOrDefault(ci => ci.ProductID == addToCartPayload.ProductID);

            if (existingCartItem != null)
            {
                // If the product already exists, update the quantity
                existingCartItem.Quantity += addToCartPayload.Quantity;
            }
            else
            {
                // If the product does not exist, create a new CartItem entity
                var newCartItem = new CartItem
                {
                    Quantity = addToCartPayload.Quantity,
                    CartID = addToCartPayload.CartID,
                    ProductID = addToCartPayload.ProductID,
                    // Assuming CartItem has a navigation property for Product
                    Product = product
                };

                // Add the new CartItem to the CartItems collection within the Cart
                user.Cart.CartItems.Add(newCartItem);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the updated user cart
            var updatedUserCart = await _context.Users
                .Include(u => u.Cart)
                .ThenInclude(c => c.CartItems)
                .FirstOrDefaultAsync(u => u.username == username);

            return Ok(updatedUserCart);
        }


        [HttpPut("{cartId}/cartitems/{itemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartId, int itemId, CartItem updatedCartItem)
        {
            if (cartId != updatedCartItem.CartID || itemId != updatedCartItem.ID)
            {
                return BadRequest();
            }

            _context.Entry(updatedCartItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(itemId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool CartItemExists(int itemId)
        {
            return _context.CartItems.Any(ci => ci.ID == itemId);
        }
        //The purpose of this 'helper' class is to 
        //define my params for adding to a user's cart...
        //seperate class used during requests...
        public class CartItemRequest
        {
            public int Quantity { get; set; }
            public int CartID { get; set; }
            public int ProductID { get; set; }
        }

        // Additional actions like updating, deleting cart items, etc. can be added here
    }
}
