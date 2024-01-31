using lostborn_backend.Helpers;
using lostborn_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // API Call to update user using Id...(UPDATE)
        [HttpPut]
        public async Task<IActionResult> PutUser(Users user)
        {
            dataContext.Entry(user).State = EntityState.Modified;

            try
            {
                // Ensure the associated Cart is attached to the context
                dataContext.Entry(user.Cart).State = EntityState.Modified;

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
                .FirstOrDefaultAsync(u => u.ID == user.ID);

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
