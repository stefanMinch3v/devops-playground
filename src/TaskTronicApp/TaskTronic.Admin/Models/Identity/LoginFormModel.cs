namespace TaskTronic.Admin.Models.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class LoginFormModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
