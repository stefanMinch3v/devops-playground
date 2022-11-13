namespace TaskTronic.Drive.Services.DapperRepo
{
    using Dapper;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class PermissionDapper : IPermissionDapper
    {
        private const string PermissionsTablename = "[dbo].[Permissions]";
        private readonly IDbConnectionFactory dbConnectionFactory;

        public PermissionDapper(IDbConnectionFactory dbConnectionFactory)
            => this.dbConnectionFactory = dbConnectionFactory;

        public async Task CreateFolderPermissionsAsync(int catalogId, int folderId, int employeeId)
        {
            var sql = $@"
                INSERT INTO {PermissionsTablename}
                (
                    CatalogId,
                    FolderId,
                    EmployeeId
                )
                VALUES
                (
                    @catalogId,
                    @folderId,
                    @employeeId
                );";

            using var db = this.dbConnectionFactory.GetSqlConnection;

            await db.ExecuteAsync(sql, new
            {
                catalogId,
                folderId,
                employeeId
            });
        }

        public async Task<IEnumerable<int>> GetUserFolderPermissionsAsync(int catalogId, int employeeId)
        {
            var sql = $@"
                SELECT FolderId FROM {PermissionsTablename}                   
                WHERE EmployeeId = @employeeId
                    AND CatalogId = @catalogId";

            using var db = this.dbConnectionFactory.GetSqlConnection;

            return (await db.QueryAsync<int>(sql, new { employeeId, catalogId })).AsList();
        }

        public async Task<bool> HasUserPermissionForFolderAsync(int catalogId, int folderId, int employeeId)
        {
            var sql = $@"
                SELECT COUNT(1) FROM {PermissionsTablename}
                WHERE EmployeeId = @employeeId
                    AND CatalogId = @catalogId
                    AND FolderId = @folderId";

            using var db = this.dbConnectionFactory.GetSqlConnection;

            return await db.QuerySingleAsync<bool>(sql, new { employeeId, catalogId, folderId });
        }
    }
}
