namespace Domain.Interfaces.Shared;

public interface IGridReader : IDisposable
{
    Task<IEnumerable<T>> ReadAsync<T>();
    Task<T> ReadFirstAsync<T>();
}
