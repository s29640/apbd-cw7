using MiniHelpdesk.Contracts;
using MiniHelpdesk.Models;

namespace MiniHelpdesk.Tests.InMemory;

public class InMemoryTicketRepository : ITicketRepository
{
    private readonly List<Ticket> _tickets = [];
    private readonly List<TicketComment> _comments = [];

    private int _nextTicketId = 1;
    private int _nextCommentId = 1;

    public Task<IReadOnlyList<Ticket>> GetAllAsync()
    {
        var result = _tickets
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<Ticket>>(result);
    }

    public Task<Ticket?> GetByIdAsync(int id)
    {
        var ticket = _tickets.FirstOrDefault(t => t.Id == id);

        return Task.FromResult(ticket);
    }

    public Task<IReadOnlyList<TicketComment>> GetCommentsAsync(int ticketId)
    {
        var result = _comments
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<TicketComment>>(result);
    }

    public Task<int> InsertTicketAsync(Ticket ticket)
    {
        ticket.Id = _nextTicketId++;

        _tickets.Add(ticket);

        return Task.FromResult(ticket.Id);
    }

    public Task<int> InsertCommentAsync(TicketComment comment)
    {
        comment.Id = _nextCommentId++;

        _comments.Add(comment);

        return Task.FromResult(comment.Id);
    }

    public Task CloseAsync(int id)
    {
        var ticket = _tickets.FirstOrDefault(t => t.Id == id);

        if (ticket is not null)
        {
            ticket.Status = TicketStatus.Closed;
        }

        return Task.CompletedTask;
    }
}