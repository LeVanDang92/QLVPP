using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OSM.Application.Abstractions.Data;
using System.Data;

namespace OSM.Infrastructure.Data
{
    public sealed class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
    {
        public IDbConnection CreateConnection()
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
            return new SqlConnection(connectionString);
        }
    }
}
