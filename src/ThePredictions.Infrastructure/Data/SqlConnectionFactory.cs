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
        var rawConnectionString = configuration.GetConnectionString("DataConnection")
            ?? throw new InvalidOperationException("Connection string 'DataConnection' not found.");

        var builder = new SqlConnectionStringBuilder(rawConnectionString)
        {
            MinPoolSize = 5,
            MaxPoolSize = 100,
            ConnectRetryCount = 3,
            ConnectRetryInterval = 10,
            LoadBalanceTimeout = 300
        };

        _connectionString = builder.ConnectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
