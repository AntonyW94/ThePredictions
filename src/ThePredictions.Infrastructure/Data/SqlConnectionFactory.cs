using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ThePredictions.Application.Configuration;
using ThePredictions.Application.Data;
using System.Data;

namespace ThePredictions.Infrastructure.Data;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration, IOptions<TimeoutSettings> timeoutSettings)
    {
        var baseConnectionString = configuration.GetConnectionString("DataConnection")
            ?? throw new InvalidOperationException("Connection string 'DataConnection' not found.");

        var builder = new SqlConnectionStringBuilder(baseConnectionString)
        {
            CommandTimeout = timeoutSettings.Value.DatabaseCommandTimeoutSeconds
        };

        _connectionString = builder.ConnectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}