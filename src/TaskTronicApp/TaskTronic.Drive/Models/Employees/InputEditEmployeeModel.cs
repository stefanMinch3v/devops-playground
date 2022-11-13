namespace TaskTronic.Drive.Models.Employees
{
    using System.ComponentModel.DataAnnotations;

    public class InputEditEmployeeModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
