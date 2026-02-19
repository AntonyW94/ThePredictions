using System.Data;

namespace ThePredictions.Application.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}