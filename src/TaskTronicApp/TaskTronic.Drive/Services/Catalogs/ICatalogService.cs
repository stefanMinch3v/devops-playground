namespace TaskTronic.Drive.Services.Catalogs
{
    using System.Threading.Tasks;

    public interface ICatalogService
    {
        Task<int> GetIdAsync(int companyDepartmentsId, int employeeId);
    }
}
