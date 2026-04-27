namespace Domain.Interfaces.Shared;

public interface IGlobalExecuters
{
    Task<int> ExecuteAsync(string spName, object? parameters = null);
    Task<T?> ExecuteScalarAsync<T>(string spName, object? parameters = null);
    Task<T?> ExecuteSingleAsync<T>(string spName, object? parameters = null);
    Task<IReadOnlyList<T>> ExecuteListAsync<T>(string spName, object? parameters = null);
    Task<T> ExecuteMultipleAsync<T>(
        string spName,
        Func<IGridReader, Task<T>> map,
        object? parameters = null);
}
