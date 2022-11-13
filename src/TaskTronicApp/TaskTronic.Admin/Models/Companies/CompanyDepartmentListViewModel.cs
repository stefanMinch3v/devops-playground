namespace TaskTronic.Admin.Models.Companies
{
    using System.Collections.Generic;

    public class CompanyDepartmentListViewModel
    {
        public int CompanyId { get; set; }

        public IReadOnlyList<OutputDepartmentServiceModel> Departments { get; set; }
    }
}
