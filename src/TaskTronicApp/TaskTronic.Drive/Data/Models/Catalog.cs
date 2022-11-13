namespace TaskTronic.Drive.Data.Models
{
    public class Catalog
    {
        public int CatalogId { get; set; }
        public int CompanyDepartmentsId { get; set; }
        public CompanyDepartments CompanyDepartments { get; set; }
    }
}