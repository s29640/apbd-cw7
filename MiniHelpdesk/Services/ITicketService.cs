using MiniHelpdesk.Models;
using MiniHelpdesk.Services.Models;

namespace MiniHelpdesk.Services;

public interface ITicketService
{
    Task<IReadOnlyList<Ticket>> GetAllAsync();

    Task<Ticket?> GetByIdAsync(int id);

    Task<IReadOnlyList<TicketComment>> GetCommentsAsync(int ticketId);

    Task<int> CreateTicketAsync(CreateTicketRequest request);

    Task CloseAsync(int id);
}