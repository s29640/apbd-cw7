using Microsoft.Data.SqlClient;
using MiniHelpdesk.Models;

namespace MiniHelpdesk.Contracts;

public interface ITicketRepository
{
    Task<IReadOnlyList<Ticket>> GetAllAsync();

    Task<Ticket?> GetByIdAsync(int id);

    Task<IReadOnlyList<TicketComment>> GetCommentsAsync(int ticketId);

    Task<int> InsertTicketAsync(Ticket ticket);

    Task<int> InsertCommentAsync(TicketComment comment);

    Task CloseAsync(int id);
}