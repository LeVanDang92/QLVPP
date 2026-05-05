using System.Data;

namespace OSM.Application.Abstractions.Data
{
    /// <summary>
    /// Query nào cần đọc nhanh thì dùng Dapper.
    /// </summary>
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
