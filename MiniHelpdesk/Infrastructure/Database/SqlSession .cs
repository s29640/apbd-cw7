using Microsoft.Data.SqlClient;

namespace MiniHelpdesk.Infrastructure.Database;

public sealed class SqlSession : ISqlSession
{
    private sealed class State
    {
        public SqlConnection? Connection { get; init; }
        public SqlTransaction? Transaction { get; init; }
    }

    private static readonly AsyncLocal<State?> Current = new();

    public SqlConnection? Connection => Current.Value?.Connection;
    public SqlTransaction? Transaction => Current.Value?.Transaction;

    public void Set(SqlConnection connection, SqlTransaction? transaction)
    {
        Current.Value = new State
        {
            Connection = connection,
            Transaction = transaction
        };
    }

    public void Clear()
    {
        Current.Value = null;
    }
}