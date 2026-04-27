using Domain.Interfaces.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infrastructure.Repositories.Shared;

internal class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    private IGlobalExecuters? _globalActions;

    public UnitOfWork(IConfiguration configuration)
    {
        // Initialize the connection immediately
        _connection = new SqlConnection(configuration.GetConnectionString("SqlConnection"));
        _connection.Open();
    }

    // Expose Connection and Transaction for external use if needed (e.g. by Repositories)
    public IDbConnection Connection => _connection;
    public IDbTransaction? Transaction => _transaction;

    // Lazy-load GlobalActions. If a transaction is started, this will be refreshed.
    public IGlobalExecuters GlobalActions => _globalActions ??= new GlobalExecuters(_connection, _transaction);

    public void BeginTransaction()
    {
        // Prevent nested transactions for simplicity
        if (_transaction != null) return;

        _transaction = _connection.BeginTransaction();

        // RE-INITIALIZE GlobalActions so it picks up the new Transaction
        _globalActions = new GlobalExecuters(_connection, _transaction);
    }

    public void Commit()
    {
        try
        {
            _transaction?.Commit();
        }
        catch
        {
            _transaction?.Rollback();
            throw;
        }
        finally
        {
            DisposeTransaction();
        }
    }

    public void Rollback()
    {
        try
        {
            _transaction?.Rollback();
        }
        finally
        {
            DisposeTransaction();
        }
    }

    private void DisposeTransaction()
    {
        _transaction?.Dispose();
        _transaction = null;
        _globalActions = null;
    }

    public async ValueTask DisposeAsync()
    {
        DisposeTransaction();

        if (_connection is IAsyncDisposable asyncConnection)
        {
            await asyncConnection.DisposeAsync();
        }
        else
        {
            _connection.Dispose();
        }
    }
}
