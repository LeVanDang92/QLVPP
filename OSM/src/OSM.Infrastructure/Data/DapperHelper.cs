using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OSM.Application.Abstractions.Data;
using OSM.Infrastructure.Persistence;
using System.Data;

namespace OSM.Infrastructure.Data
{
    public class DapperHelper : IDapperHelper
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly ApplicationDbContext _dbContext;

        public DapperHelper(
            ISqlConnectionFactory sqlConnectionFactory,
            ApplicationDbContext dbContext)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _dbContext = dbContext;
        }

        public async Task<int> ExecuteAsync(
            string sql,
            object param = null,
            IDbTransaction transaction = null)
        {
            if (transaction != null)
            {
                return await transaction.Connection.ExecuteAsync(sql, param, transaction);
            }

            var currentTransaction = _dbContext.Database.CurrentTransaction;

            if (currentTransaction != null)
            {
                var connection = _dbContext.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                return await connection.ExecuteAsync(
                    sql,
                    param,
                    currentTransaction.GetDbTransaction());
            }

            using var conn = _sqlConnectionFactory.CreateConnection();
            return await conn.ExecuteAsync(sql, param);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null)
        {
            if (transaction != null)
            {
                return await transaction.Connection.QueryAsync<T>(sql, param, transaction);
            }

            var currentTransaction = _dbContext.Database.CurrentTransaction;

            if (currentTransaction != null)
            {
                var connection = _dbContext.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                return await connection.QueryAsync<T>(
                    sql,
                    param,
                    currentTransaction.GetDbTransaction());
            }

            using var conn = _sqlConnectionFactory.CreateConnection();
            return await conn.QueryAsync<T>(sql, param);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(
            string sql,
            object param = null,
            IDbTransaction transaction = null)
        {
            if (transaction != null)
            {
                return await transaction.Connection.QueryFirstOrDefaultAsync<T>(
                    sql,
                    param,
                    transaction);
            }

            var currentTransaction = _dbContext.Database.CurrentTransaction;

            if (currentTransaction != null)
            {
                var connection = _dbContext.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                return await connection.QueryFirstOrDefaultAsync<T>(
                    sql,
                    param,
                    currentTransaction.GetDbTransaction());
            }

            using var conn = _sqlConnectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<T>(sql, param);
        }

        public async Task<IEnumerable<T>> QueryProcAsync<T>(
            string procedureName,
            object param = null,
            IDbTransaction transaction = null)
        {
            if (transaction != null)
            {
                return await transaction.Connection.QueryAsync<T>(
                    procedureName,
                    param,
                    transaction,
                    commandType: CommandType.StoredProcedure);
            }

            var currentTransaction = _dbContext.Database.CurrentTransaction;

            if (currentTransaction != null)
            {
                var connection = _dbContext.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                return await connection.QueryAsync<T>(
                    procedureName,
                    param,
                    currentTransaction.GetDbTransaction(),
                    commandType: CommandType.StoredProcedure);
            }

            using var conn = _sqlConnectionFactory.CreateConnection();

            return await conn.QueryAsync<T>(
                procedureName,
                param,
                commandType: CommandType.StoredProcedure);
        }
    }
}