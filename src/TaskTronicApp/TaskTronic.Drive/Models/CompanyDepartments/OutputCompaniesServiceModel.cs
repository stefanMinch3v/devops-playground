namespace TaskTronic.Drive.Models.CompanyDepartments
{
    using System.Collections.Generic;

    public class OutputCompaniesServiceModel
    {
        public IReadOnlyList<OutputCompanyDepartmentsServiceModel> Companies { get; set; } 

        public OutputSelectedCompanyServiceModel SelectedData { get; set; }
    }
}
