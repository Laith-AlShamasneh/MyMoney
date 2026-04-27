using Dapper;
using Domain.Helpers;
using Domain.Interfaces.Shared;
using Microsoft.Data.SqlClient;
using System.Data;
using static Dapper.SqlMapper;

namespace Infrastructure.Repositories.Shared;

internal class GlobalExecuters(IDbConnection connection, IDbTransaction? transaction) : IGlobalExecuters
{
    private IDbConnection Cn => connection;
    private IDbTransaction? Tx => transaction;

    private static async Task<TResult> HandleSqlExceptions<TResult>(Func<Task<TResult>> action)
    {
        try
        {
            return await action();
        }
        catch (SqlException ex) when (ex.Number >= 50000)
        {
            throw new SqlExceptionHelper(ex.Number, ex.Message, ex);
        }
    }

    public async Task<int> ExecuteAsync(string spName, object? parameters = null)
    {
        return await HandleSqlExceptions(async () =>
            await Cn.ExecuteAsync(spName, parameters, Tx, commandType: CommandType.StoredProcedure));
    }

    public async Task<T?> ExecuteScalarAsync<T>(string spName, object? parameters = null)
    {
        return await HandleSqlExceptions(async () =>
            await Cn.ExecuteScalarAsync<T>(spName, parameters, Tx, commandType: CommandType.StoredProcedure));
    }

    public async Task<T?> ExecuteSingleAsync<T>(string spName, object? parameters = null)
    {
        return await HandleSqlExceptions(async () =>
            await Cn.QueryFirstOrDefaultAsync<T>(spName, parameters, Tx, commandType: CommandType.StoredProcedure));
    }

    public async Task<IReadOnlyList<T>> ExecuteListAsync<T>(string spName, object? parameters = null)
    {
        return await HandleSqlExceptions<IReadOnlyList<T>>(async () =>
        {
            var result = await Cn.QueryAsync<T>(spName, parameters, Tx, commandType: CommandType.StoredProcedure);
            return [.. result];
        });
    }

    public async Task<T> ExecuteMultipleAsync<T>(string spName, Func<IGridReader, Task<T>> map, object? parameters = null)
    {
        return await HandleSqlExceptions(async () =>
        {
            using var multi = await Cn.QueryMultipleAsync(spName, parameters, Tx, commandType: CommandType.StoredProcedure);
            var wrapper = new DapperGridReaderWrapper(multi);
            return await map(wrapper);
        });
    }
}

internal class DapperGridReaderWrapper(GridReader reader) : IGridReader
{
    public async Task<IEnumerable<T>> ReadAsync<T>() => await reader.ReadAsync<T>();
    public async Task<T> ReadFirstAsync<T>() => await reader.ReadFirstAsync<T>();
    public void Dispose() { }
}