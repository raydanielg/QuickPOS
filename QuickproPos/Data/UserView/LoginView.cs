using System.ComponentModel.DataAnnotations;

namespace QuickproPos.Data.UserView
{
    public class LoginView
    {
        public int UserId { get; set; }

        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; } // Store hashed password
        public string Role { get; set; } // User roles, e.g., Admin, User, etc.
    }
}
