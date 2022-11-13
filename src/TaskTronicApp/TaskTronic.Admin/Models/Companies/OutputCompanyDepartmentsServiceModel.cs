namespace TaskTronic.Admin.Models.Companies
{
    using System.Collections.Generic;

    public class OutputCompanyDepartmentsServiceModel
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }

        public IReadOnlyList<OutputDepartmentMapServiceModel> Departments { get; set; }
    }
}
