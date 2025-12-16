using System.ComponentModel.DataAnnotations;

namespace SecurePostManagerApi.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)] // Exige un mot de passe d'au moins 6 caractères
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}