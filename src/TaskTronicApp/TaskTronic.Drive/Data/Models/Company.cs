namespace TaskTronic.Drive.Data.Models
{
    using System.Collections.Generic;

    public class Company
    {
        public int CompanyId { get; set; }

        public string Name { get; set; }

        public HashSet<CompanyDepartments> Departments { get; set; } = new HashSet<CompanyDepartments>();
    }
}
