using Microsoft.Data.SqlClient;
using MiniHelpdesk.Contracts;
using System.Data.Common;
using System.Transactions;

namespace MiniHelpdesk.Infrastructure.Database;

public class SqlUnitOfWork : IUnitOfWork
{
    private readonly IConfiguration _configuration;
    private readonly ISqlSession _session;

    public SqlUnitOfWork(IConfiguration configuration, ISqlSession session)
    {
        _configuration = configuration;
        _session = session;
    }

    public async Task ExecuteAsync(Func<Task> action)
    {
        await using var connection =
            new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

        await connection.OpenAsync();

        _session.Set(connection, null);

        try
        {
            await action();
        }
        finally
        {
            _session.Clear();
        }
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        await using var connection =
            new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

        await connection.OpenAsync();

        _session.Set(connection, null);

        try
        {
            var result = await action();
            return result;
        }
        finally
        {
            _session.Clear();
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await using var connection =
            new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

        await connection.OpenAsync();

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        _session.Set(connection, transaction);

        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            _session.Clear();
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        await using var connection =
            new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

        await connection.OpenAsync();

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        _session.Set(connection, transaction);

        try
        {
            var result = await action();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            _session.Clear();
        }
    }
}