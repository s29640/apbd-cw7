using MiniHelpdesk.Contracts;

namespace MiniHelpdesk.Tests.InMemory;

public class InMemoryUnitOfWork : IUnitOfWork
{
    public Task ExecuteAsync(Func<Task> action)
    {
        return action();
    }

    public Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        return action();
    }

    public Task ExecuteInTransactionAsync(Func<Task> action)
    {
        return action();
    }

    public Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        return action();
    }
}