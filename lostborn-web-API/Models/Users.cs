using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace lostborn_backend.Models
{
    public class Users
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string firstName { get; set; }

        [Required]
        public string lastName { get; set; }

        [Required]
        public string streetAddress { get; set; }

        [Required]
        public string city { get; set; }

        [Required]
        public string zipCode { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public decimal userBalance { get; set; }



        // Navigation property to represent the one-to-one relationship with Cart
        public Carts Cart { get; set; }

        public Users() { }

        public Users(int id, string firstName, string lastName, string streetAddress, string city, string zipCode, string email, string username, string password, string Role,decimal userBalance)
        {
            ID = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.streetAddress = streetAddress;
            this.city = city;
            this.zipCode = zipCode;
            this.email = email;
            this.username = username;
            this.password = password;
            this.Role = Role;
            this.userBalance = userBalance;

        }
    }
}
