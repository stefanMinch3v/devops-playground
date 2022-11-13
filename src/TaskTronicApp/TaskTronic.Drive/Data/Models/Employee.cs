namespace TaskTronic.Drive.Data.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }

        public int CompanyDepartmentsId { get; set; }
        public CompanyDepartments CompanyDepartments { get; set; }
    }
}
