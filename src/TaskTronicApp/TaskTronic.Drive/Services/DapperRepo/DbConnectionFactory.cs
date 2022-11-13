namespace TaskTronic.Drive.Services.DapperRepo
{
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string dbConnectionString;

        public DbConnectionFactory(IConfiguration configuration)
            => this.dbConnectionString = configuration["ConnectionStrings:DefaultConnection"];

        public SqlConnection GetSqlConnection
            => new SqlConnection(this.dbConnectionString);
    }
}
