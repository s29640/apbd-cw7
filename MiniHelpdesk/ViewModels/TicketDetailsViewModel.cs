using MiniHelpdesk.Models;

namespace MiniHelpdesk.ViewModels;

public class TicketDetailsViewModel
{
    public Ticket Ticket { get; set; } = default!;

    public IReadOnlyList<TicketComment> Comments { get; set; } = [];
}