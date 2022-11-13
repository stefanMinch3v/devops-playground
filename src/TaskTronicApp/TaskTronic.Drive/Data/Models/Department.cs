namespace TaskTronic.Drive.Data.Models
{
    using System.Collections.Generic;

    public class Department
    {
        public int DepartmentId { get; set; }

        public string Name { get; set; }

        public HashSet<CompanyDepartments> Companies { get; set; } = new HashSet<CompanyDepartments>();
    }
}
