namespace TaskTronic.Drive.Data.Models
{
    public class CompanyDepartments
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public int DepartmentId { get; set; }

        public Company Company { get; set; }

        public Department Department { get; set; }
    }
}
