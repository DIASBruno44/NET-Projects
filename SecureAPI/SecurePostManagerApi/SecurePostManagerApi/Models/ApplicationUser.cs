using Microsoft.AspNetCore.Identity;

namespace SecurePostManagerApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime AccountCreationDate { get; set; } = DateTime.UtcNow;

    }
}