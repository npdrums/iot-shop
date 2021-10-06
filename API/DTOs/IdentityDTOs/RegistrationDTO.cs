using System.ComponentModel.DataAnnotations;

namespace API.DTOs.IdentityDTOs
{
    public class RegistrationDTO
    {   [Required]
        public string DisplayName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        // TODO: Fix Validation
        // [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$", 
        // ErrorMessage = "Password must have 1 number, 1 Uppercase letter and at least 8 characters")]
        public string Password { get; set; }
    }
} 