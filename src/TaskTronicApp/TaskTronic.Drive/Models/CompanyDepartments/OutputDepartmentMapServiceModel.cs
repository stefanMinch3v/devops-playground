namespace TaskTronic.Drive.Models.CompanyDepartments
{
    using TaskTronic.Models;
    using Data.Models;
    using AutoMapper;

    public class OutputDepartmentMapServiceModel : IMapFrom<CompanyDepartments>
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public void Mapping(Profile mapper)
            => mapper
                .CreateMap<CompanyDepartments, OutputDepartmentMapServiceModel>()
                .ForMember(d => d.DepartmentName, cfg => cfg.MapFrom(src => src.Department.Name));
    }
}
