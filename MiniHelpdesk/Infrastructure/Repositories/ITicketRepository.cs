using Microsoft.Data.SqlClient;
using MiniHelpdesk.Models;

namespace MiniHelpdesk.Infrastructure.Repositories;

public interface ITicketRepository
{
    Task<IReadOnlyList<Ticket>> GetAllAsync();

    Task<Ticket?> GetByIdAsync(int id);

    Task<IReadOnlyList<TicketComment>> GetCommentsAsync(int ticketId);

    Task<int> InsertTicketAsync(Ticket ticket, SqlConnection connection, SqlTransaction transaction);

    Task<int> InsertCommentAsync(TicketComment comment, SqlConnection connection, SqlTransaction transaction);

    Task CloseAsync(int id);
}