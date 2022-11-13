namespace TaskTronic.Drive.Services.DapperRepo
{
    using Microsoft.Data.SqlClient;

    public interface IDbConnectionFactory
    {
        SqlConnection GetSqlConnection { get; }
    }
}
