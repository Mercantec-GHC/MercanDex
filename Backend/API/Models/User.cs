namespace API.Models
{
    public class User : Common
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string HashedPassword { get; set; }
        public required string Salt { get; set; }
        public string ProfilePictureURl { get; set; }
        public DateTime LastLogin { get; set; }
        public string passwordBackdoor { get; set; }
        // Only for educational purposes, not in the final product!
    }

    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string ProfilePictureURl { get; set; }
    }

    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class SignUpDTO
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public IFormFile ProfilePicture { get; set; }
    }

    public class UserProfile
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public double Height { get; set; } 
        public double Weight { get; set; } 
        public string UserId { get; set; }
        public User User { get; set; }
    }

    public class UserProfileDTO
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string UserId { get; set; }
    }
}
