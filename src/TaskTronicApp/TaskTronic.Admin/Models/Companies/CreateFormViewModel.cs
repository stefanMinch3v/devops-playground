namespace TaskTronic.Admin.Models.Companies
{
    using System.ComponentModel.DataAnnotations;

    public class CreateFormViewModel
    {
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
