using lostborn_backend.Helpers;
using lostborn_backend.Models;
using lostborn_web_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace lostborn_backend.Controllers
{
    [Authorize]
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
            var updatedMember = await _context.Products.FindAsync(member.ID);

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
            var product = await _context.Products.FindAsync(addToCartPayload.ProductID);

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
                try
                {
                    user.Cart.CartItems.Add(newCartItem);
                }
                catch(Exception ex)
                {
                    Exception exemption = ex;
                    return BadRequest(new { message = ex.Message });
                }
                
            }
            try
            {
                var result = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Exception exemption = ex;
                return BadRequest(new { message = ex.Message });
            }

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
        
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            int userId;

            if (int.TryParse(userIdClaim, out userId) && userId != model.UserId)
            { 
                // Handle the case where the user ID is not valid or does not match
                return Unauthorized("Invalid user ID or user ID does not match.");
            }

            // Attempt to retrieve the user's active cart
            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                     .ThenInclude(ci => ci.Product)  // Assuming you want product details too
                                     .FirstOrDefaultAsync(c => c.UserID == model.UserId);

            if (cart == null)
            {
                return NotFound(new { Message = "No active cart found for this user." });
            }

            // Now construct the payment request
            PaymentRequest paymentRequest = new PaymentRequest
            {
                CardNumber = model.CardNumber,
                CardExpiry = model.CardExpiry,
                Cvv = model.Cvv,
                Amount = cart.CartItems.Sum(item => item.Quantity * item.Product.Price)  // Recalculate to prevent tampering
            };

            //For future reference, the next PaymentResponse would be pointing to a service with an actual payment provider
            //Then we would read its response and act accordingly. 

            // Process the payment
            PaymentResponse paymentResponse = new MockPaymentService().ProcessPayment(paymentRequest);

            if (paymentResponse.IsSuccess)
            {
                // Construct a new invoice
                Invoice newInvoice = new Invoice
                {
                    UserID = model.UserId,
                    InvoiceDate = DateTime.Now,
                    TotalAmount = cart.CartItems.Sum(item => item.Quantity * item.Product.Price),
                    InvoiceNumber = GenerateInvoiceNumber(),
                    PaymentStatus = "Paid",
                    TransactionId = paymentResponse.TransactionId,
                    InvoiceItems = cart.CartItems.Select(item => new InvoiceItem
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price
                    }).ToList()
                };

                // Save the new invoice first to ensure the InvoiceID is generated
                _context.Invoices.Add(newInvoice);
                await _context.SaveChangesAsync();  // Save here to generate the InvoiceID

                // Now that the InvoiceID is available, log each transaction
                foreach (var cartItem in cart.CartItems)
                {
                    // Fetch the product from the database to update its InStock value
                    var product = await _context.Products.FindAsync(cartItem.ProductID);
                    if (product != null)
                    {
                        // Reduce the stock by the quantity purchased
                        product.InStock -= cartItem.Quantity;

                        // Update the product stock in the database
                        _context.Products.Update(product);
                    }

                    var transaction = new Transaction
                    {
                        ProductID = cartItem.ProductID,
                        QuantityChange = -cartItem.Quantity,
                        TransactionType = "SALE",
                        TransactionDate = DateTime.Now,
                        InvoiceID = newInvoice.InvoiceID  // Use the saved InvoiceID
                    };

                    _context.Transactions.Add(transaction);
                }

                // Save the transactions to the database
                await _context.SaveChangesAsync();

                // Clear the cart items after successful checkout
                _context.CartItems.RemoveRange(cart.CartItems);

                // Save the changes to empty the cart
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Payment and invoice processed successfully", Invoice = newInvoice });
            }
            else
            {
                return BadRequest(new { Message = "Payment Failed", Error = paymentResponse.ErrorMessage });
            }

        }

        private string GenerateInvoiceNumber()
        {
            var datePart = DateTime.Now.ToString("yyyyMMdd"); // e.g., 20230917
            var randomPart = new Random().Next(100, 999); // e.g., between 100 and 999
            return $"INV-{datePart}-{randomPart}";
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
    }
}
