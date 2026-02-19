using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ThePredictions.Application.Data;
using System.Data;

namespace ThePredictions.Infrastructure.Data;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DataConnection") ?? throw new InvalidOperationException("Connection string 'DataConnection' not found.");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}