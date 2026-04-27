using Domain.Interfaces.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infrastructure.Repositories.Shared;

internal class UnitOfWork(IConfiguration configuration) : IUnitOfWork
{
    private readonly string _connectionString = configuration.GetConnectionString("SqlConnection")
            ?? throw new InvalidOperationException("Connection string 'SqlConnection' not found in configuration.");
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    private IGlobalExecuters? _globalActions;

    public IDbConnection Connection
    {
        get
        {
            // Lazily instantiate and open the connection only when accessed
            _connection ??= new SqlConnection(_connectionString);
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection;
        }
    }

    public IDbTransaction? Transaction => _transaction;

    // Passing the 'Connection' property ensures it opens if this is the first DB access
    public IGlobalExecuters GlobalActions => _globalActions ??= new GlobalExecuters(Connection, _transaction);

    public void BeginTransaction()
    {
        // Prevent nested transactions for simplicity
        if (_transaction is not null) return;

        // Accessing the 'Connection' property guarantees the DB is open before beginning the transaction
        _transaction = Connection.BeginTransaction();

        // RE-INITIALIZE GlobalActions so it picks up the newly created Transaction
        _globalActions = new GlobalExecuters(Connection, _transaction);
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
        _globalActions = null; // Reset so the next DB call creates a fresh executor without a dead transaction
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
            _connection?.Dispose();
        }
    }
}