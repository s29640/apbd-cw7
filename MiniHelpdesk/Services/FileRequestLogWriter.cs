namespace MiniHelpdesk.Services;

public class FileRequestLogWriter : IRequestLogWriter
{
    private readonly IWebHostEnvironment _environment;
    private static readonly SemaphoreSlim Lock = new(1, 1);

    public FileRequestLogWriter(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task WriteAsync(string message)
    {
        var logsDirectory = Path.Combine(_environment.ContentRootPath, "Logs");

        Directory.CreateDirectory(logsDirectory);

        var filePath = Path.Combine(
            logsDirectory,
            $"requests-{DateTime.Now:yyyy-MM-dd}.log");

        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}{Environment.NewLine}";

        await Lock.WaitAsync();

        try
        {
            await File.AppendAllTextAsync(filePath, line);
        }
        finally
        {
            Lock.Release();
        }
    }
}