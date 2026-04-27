using System.Data;

namespace Domain.Interfaces.Shared;

public interface IUnitOfWork : IAsyncDisposable
{
    void BeginTransaction();
    void Commit();
    void Rollback();

    IDbConnection Connection { get; }
    IDbTransaction? Transaction { get; }
    IGlobalExecuters GlobalActions { get; }
}
