namespace MiniHelpdesk.Services;

public interface IRequestLogWriter
{
    Task WriteAsync(string message);
}