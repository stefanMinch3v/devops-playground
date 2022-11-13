namespace TaskTronic.Drive.Data.Models
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public int CatalogId { get; set; }
        public int FolderId { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
