namespace TaskTronic.Admin.Models.Employees
{
    public class OutputEmployeeDetailsServiceModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int TotalFiles { get; set; }

        public int TotalFolders { get; set; }
    }
}
