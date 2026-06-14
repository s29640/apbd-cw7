using Microsoft.Data.SqlClient;

namespace MiniHelpdesk.Infrastructure.Database;

public interface ISqlSession
{
    SqlConnection? Connection { get; }
    SqlTransaction? Transaction { get; }

    void Set(SqlConnection connection, SqlTransaction? transaction);
    void Clear();
}