using Microsoft.Extensions.Configuration;

namespace MiniHelpdesk.Tests;

internal static class TestConfiguration
{
    public static IConfiguration Create()
    {
        return new ConfigurationBuilder()
            .SetBasePath(GetSolutionRoot())
            .AddJsonFile("MiniHelpdesk/appsettings.json", optional: false)
            .Build();
    }

    private static string GetSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null &&
               !File.Exists(Path.Combine(directory.FullName, "MiniHelpdesk.slnx")))
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