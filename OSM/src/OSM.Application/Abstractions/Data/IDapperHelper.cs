using System.Data;

namespace OSM.Application.Abstractions.Data
{
    public interface IDapperHelper
    {
        /// <summary>
        /// Chạy câu lệnh không trả về dữ liệu (Insert, Update, Delete)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null);

        /// <summary>
        ///  Lấy 1 bản ghi duy nhất
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null);

        /// <summary>
        /// Lấy danh sách bản ghi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null);

        /// <summary>
        /// Thực thi Stored Procedure trả về danh sách
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryProcAsync<T>(string procedureName, object param = null, IDbTransaction transaction = null);
    }
}
