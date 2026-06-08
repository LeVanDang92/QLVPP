using Dapper;
using OSM.Application.Abstractions.Data;
using System.Data;

namespace OSM.Infrastructure.Data
{
    public class DapperHelper : IDapperHelper
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public DapperHelper(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null)
        {
            // Nếu có transaction truyền vào, dùng chung connection của transaction đó
            if (transaction != null)
            {
                return await transaction.Connection.ExecuteAsync(sql, param, transaction);
            }

            // Nếu không, tự tạo connection mới và giải phóng sau khi dùng xong
            using var conn = _sqlConnectionFactory.CreateConnection();
            return await conn.ExecuteAsync(sql, param);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            if (transaction != null)
            {
                return await transaction.Connection.QueryAsync<T>(sql, param, transaction);
            }

            using var conn = _sqlConnectionFactory.CreateConnection();
            return await conn.QueryAsync<T>(sql, param);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            if (transaction != null)
            {
                return await transaction.Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
            }

            using var conn = _sqlConnectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        public async Task<IEnumerable<T>> QueryProcAsync<T>(string procedureName, object param = null, IDbTransaction transaction = null)
        {
            if (transaction != null)
            {
                return await transaction.Connection.QueryAsync<T>(procedureName, param, transaction, commandType: CommandType.StoredProcedure);
            }

            using var conn = _sqlConnectionFactory.CreateConnection();
            return await conn.QueryAsync<T>(procedureName, param, commandType: CommandType.StoredProcedure);
        }
    }
}
