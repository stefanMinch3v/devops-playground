namespace TaskTronic.Identity.Models
{
    using System.ComponentModel.DataAnnotations;

    public class InputRegisterModel
    {
        [Required]
        [EmailAddress]
        [MinLength(3)]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
