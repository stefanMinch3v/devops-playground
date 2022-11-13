namespace TaskTronic.Drive.Models.Employees
{
    using AutoMapper;
    using Data.Models;

    public class OutputEmployeeDetailsServiceModel : OutputEmployeeModel
    {
        public int TotalFolders { get; set; }

        public int TotalFiles { get; set; }

        public void Mapping(Profile mapper)
            => mapper
                .CreateMap<Employee, OutputEmployeeDetailsServiceModel>()
                .IncludeBase<Employee, OutputEmployeeModel>();
    }
}
