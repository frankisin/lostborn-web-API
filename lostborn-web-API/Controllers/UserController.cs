using lostborn_backend.Helpers;
using lostborn_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace lostborn_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly DataContext dataContext;

        public UserController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        // API Call to get all users...
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await dataContext.Users.ToListAsync();
            return Ok(users);
        }
        [HttpGet("Balance/{ID}")]
        public async Task<IActionResult> GetUserBalance(int ID)
        {
            var user = await dataContext.Users.FindAsync(ID);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            return Ok(new { user.userBalance });
        }
        [HttpPut("UpdateBalance/{id}")]
        public async Task<IActionResult> UpdateUserBalance(int id, [FromBody] decimal balanceChange)
        {
            // Find the user by ID
            var user = await dataContext.Users.FirstOrDefaultAsync(u => u.ID == id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Increment or decrement the balance
            user.userBalance += balanceChange;

            try
            {
                // Save changes to the database
                await dataContext.SaveChangesAsync();
                return Ok(new { message = "Balance updated successfully", userBalance = user.userBalance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("{ID}")]
        public async Task<ActionResult<Users>> GetUser(int ID)
        {
            var user = await dataContext.Users
                .Include(u => u.Cart)
                    .ThenInclude(c => c.CartItems)  // Include CartItems within Cart
                .FirstOrDefaultAsync(u => u.ID == ID);

            if (user == null)
            {
                return NotFound();
            }

            // If the user doesn't have a cart, create one
            if (user.Cart == null)
            {
                var newCart = new Carts { UserID = user.ID };
                dataContext.Carts.Add(newCart);
                await dataContext.SaveChangesAsync();
                user.Cart = newCart;
            }

            return user;
        }
        [HttpGet("ByUsername/{username}/Cart")]
        public async Task<ActionResult<Users>> GetUserCart(string username)
        {
            var user = await dataContext.Users
                .Include(u => u.Cart)
                    .ThenInclude(c => c.CartItems)  // Include CartItems within Cart
                        .ThenInclude(ci => ci.Product)  // Include Product details within CartItems
                .FirstOrDefaultAsync(u => u.username == username);

            if (user == null)
            {
                return NotFound();
            }

            // If the user doesn't have a cart, create one
            if (user.Cart == null)
            {
                var newCart = new Carts { UserID = user.ID };
                dataContext.Carts.Add(newCart);
                await dataContext.SaveChangesAsync();
                user.Cart = newCart;
            }

            return user;
        }
        // API Call to add a new user entry...(CREATE)
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] Users user)
        {
            await dataContext.Users.AddAsync(user);
            await dataContext.SaveChangesAsync();

            return CreatedAtAction(nameof(AddUser), user);
        }

        [HttpPut]
        public async Task<IActionResult> PutUser(UpdateUserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("Invalid user data");
            }

            var existingUser = await dataContext.Users.FindAsync(userDto.ID);

            if (existingUser == null)
            {
                return NotFound(); // Or handle the case where the user with the specified ID is not found
            }

            // Update the properties of the existing user with the values from the DTO
            existingUser.firstName = userDto.firstName;
            existingUser.lastName = userDto.lastName;
            existingUser.streetAddress = userDto.streetAddress;
            existingUser.city = userDto.city;
            existingUser.zipCode = userDto.zipCode;
            existingUser.email = userDto.email;
            existingUser.username = userDto.username;
            existingUser.password = userDto.password;
            existingUser.Role = userDto.Role;

            try
            {
                await dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency exception if needed
                throw;
            }

            // Fetch the updated data from the database, including associated carts
            var updatedUser = await dataContext.Users
                .Include(u => u.Cart) // Ensure Cart is included in the query
                .FirstOrDefaultAsync(u => u.ID == existingUser.ID);

            // You can create a custom response object or use an anonymous object
            var response = new
            {
                Message = "Record Updated successfully",
                UpdatedData = updatedUser
            };

            // Return the custom response
            return Ok(response);
        }


        // Helper function to check if a user is present..
        private bool UserExists(int id)
        {
            return dataContext.Users.Any(e => e.ID == id);
        }
        
        public class UpdateUserDto
        {
            public int ID { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string streetAddress { get; set; }
            public string city { get; set; }
            public string zipCode { get; set; }
            public string email { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string Role { get; set; }
        }

        // API Call to delete a user using Id...(DELETE)
        [HttpDelete("{ID}")]
        public async Task<IActionResult> DeleteUser(int ID)
        {
            var user = await dataContext.Users.FindAsync(ID);

            if (user == null)
            {
                return NotFound();
            }

            dataContext.Users.Remove(user);
            await dataContext.SaveChangesAsync();

            // You can create a custom response object or use an anonymous object
            var response = new
            {
                Message = "Record Deleted successfully",
                DeletedData = user
            };

            // Return the custom response
            return Ok(response);
        }
    }
}
