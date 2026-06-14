namespace MiniHelpdesk.Contracts;

public interface IUnitOfWork
{
    Task ExecuteAsync(Func<Task> action);
    Task<T> ExecuteAsync<T>(Func<Task<T>> action);

    Task ExecuteInTransactionAsync(Func<Task> action);
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
}