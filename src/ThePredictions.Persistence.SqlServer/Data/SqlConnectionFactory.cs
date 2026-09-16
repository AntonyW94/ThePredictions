using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ThePredictions.Application.Configuration;
using ThePredictions.Application.Data;
using System.Data;

namespace ThePredictions.Persistence.SqlServer.Data;

[ExcludeFromCodeCoverage(Justification = "Database plumbing: connection, transaction and type-handler wiring with no branching logic of its own.")]
public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration, IOptions<TimeoutSettings> timeoutSettings)
    {
        var rawConnectionString = configuration.GetConnectionString("DataConnection")
            ?? throw new InvalidOperationException("Connection string 'DataConnection' not found.");

        // MinPoolSize is sized for the dashboard rather than for a single read. Its tiles are separate
        // endpoints that the page requests at once, and a single load was measured issuing more than
        // fifteen reads inside 36ms - each taking its own connection. Keeping five warm meant ten of
        // them opened a fresh one, and a fresh one here is a TCP connect, a TLS handshake and a SQL
        // login to a remote host: measured at 300-750ms, against queries that then ran in 2-40ms.
        //
        // LoadBalanceTimeout is deliberately absent. It is the "Connection Lifetime" keyword, which
        // destroys a pooled connection once it is older than the given seconds, and it exists to let
        // connections drift back across the nodes of a *clustered* server. This instance is not
        // clustered, so the previous value of 300 bought nothing and cost the pool: every connection
        // was thrown away five minutes after it was opened, so the handshake above was paid again and
        // again, and MinPoolSize could never keep anything warm. Omitting it leaves the default of 0,
        // meaning pooled connections live until they go idle.
        var builder = new SqlConnectionStringBuilder(rawConnectionString)
        {
            CommandTimeout = timeoutSettings.Value.DatabaseCommandTimeoutSeconds,
            MinPoolSize = 25,
            MaxPoolSize = 100,
            ConnectRetryCount = 3,
            ConnectRetryInterval = 10
        };

        _connectionString = builder.ConnectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
