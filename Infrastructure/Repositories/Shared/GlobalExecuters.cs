using Domain.Interfaces.Shared;
using System.Data;
using static Dapper.SqlMapper;

namespace Infrastructure.Repositories.Shared;

internal class GlobalExecuters(IDbConnection connection, IDbTransaction? transaction) : IGlobalExecuters
{
    private IDbConnection Cn => connection;
    private IDbTransaction? Tx => transaction;

    public async Task<int> ExecuteAsync(string spName, object? parameters = null)
    {
        return await Cn.ExecuteAsync(spName, parameters, Tx, commandType: CommandType.StoredProcedure);
    }

    public async Task<T?> ExecuteScalarAsync<T>(string spName, object? parameters = null)
    {
        return await Cn.ExecuteScalarAsync<T>(spName, parameters, Tx, commandType: CommandType.StoredProcedure);
    }

    public async Task<T?> ExecuteSingleAsync<T>(string spName, object? parameters = null)
    {
        return await Cn.QueryFirstOrDefaultAsync<T>(spName, parameters, Tx, commandType: CommandType.StoredProcedure);
    }

    public async Task<IReadOnlyList<T>> ExecuteListAsync<T>(string spName, object? parameters = null)
    {
        var result = await Cn.QueryAsync<T>(spName, parameters, Tx, commandType: CommandType.StoredProcedure);
        return [.. result];
    }

    public async Task<T> ExecuteMultipleAsync<T>(string spName, Func<IGridReader, Task<T>> map, object? parameters = null)
    {
        using var multi = await Cn.QueryMultipleAsync(spName, parameters, Tx, commandType: CommandType.StoredProcedure);

        var wrapper = new DapperGridReaderWrapper(multi);

        return await map(wrapper);
    }
}

internal class DapperGridReaderWrapper(GridReader reader) : IGridReader
{
    public async Task<IEnumerable<T>> ReadAsync<T>() => await reader.ReadAsync<T>();
    public async Task<T> ReadFirstAsync<T>() => await reader.ReadFirstAsync<T>();
    public void Dispose() { }
}