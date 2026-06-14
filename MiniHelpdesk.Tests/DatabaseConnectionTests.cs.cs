using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace MiniHelpdesk.Tests;

public class DatabaseConnectionTests
{
    [Fact]
    public async Task Should_Open_Connection_To_Database()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(GetSolutionRoot())
            .AddJsonFile("MiniHelpdesk/appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Assert.False(string.IsNullOrWhiteSpace(connectionString));

        await using var connection = new SqlConnection(connectionString);

        await connection.OpenAsync();

        Assert.Equal(System.Data.ConnectionState.Open, connection.State);
    }

    private static string GetSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MiniHelpdesk.slnx")))
        {
            directory = directory.Parent;
        }

        if (directory is null)
        {
            throw new InvalidOperationException("Nie znaleziono katalogu solution.");
        }

        return directory.FullName;
    }
}