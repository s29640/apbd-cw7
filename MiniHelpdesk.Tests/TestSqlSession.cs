using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MiniHelpdesk.Infrastructure.Database;

namespace MiniHelpdesk.Tests;

internal static class TestSqlSession
{
    public static async Task<ISqlSession> Create()
    {
        var session = new SqlSession();

        var connection = new SqlConnection(TestConfiguration.Create().GetConnectionString("DefaultConnection"));

        await connection.OpenAsync();

        var transaction = (SqlTransaction)await connection!.BeginTransactionAsync();

        session.Set(connection, transaction);

        return session;
    }

}