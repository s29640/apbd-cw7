using MiniHelpdesk.Contracts;
using MiniHelpdesk.Models;
using MiniHelpdesk.Services.Models;

namespace MiniHelpdesk.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(ITicketRepository ticketRepository, IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyList<Ticket>> GetAllAsync()
    {
        return _unitOfWork.ExecuteAsync(() => _ticketRepository.GetAllAsync());
    }

    public Task<Ticket?> GetByIdAsync(int id)
    {
        return _unitOfWork.ExecuteAsync(() => _ticketRepository.GetByIdAsync(id));
    }

    public Task<IReadOnlyList<TicketComment>> GetCommentsAsync(int ticketId)
    {
        return _unitOfWork.ExecuteAsync(() => _ticketRepository.GetCommentsAsync(ticketId));
    }

    public Task<int> CreateTicketAsync(CreateTicketRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                "Ticket title is required. Please enter a title before creating the ticket.",
                nameof(request.Title));
        }

        if (string.IsNullOrWhiteSpace(request.FirstCommentContent))
        {
            throw new ArgumentException(
                "First comment content is required. Please enter an initial comment before creating the ticket.",
                nameof(request.FirstCommentContent));
        }

        return _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var now = DateTime.UtcNow;

            var ticket = new Ticket
            {
                Title = request.Title,
                Description = request.Description,
                Status = TicketStatus.Open,
                CreatedAt = now
            };

            var ticketId = await _ticketRepository.InsertTicketAsync(ticket);

            var comment = new TicketComment
            {
                TicketId = ticketId,
                Author = request.Author,
                Content = request.FirstCommentContent,
                CreatedAt = now
            };

            await _ticketRepository.InsertCommentAsync(comment);

            return ticketId;
        });
    }

    public Task CloseAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Nieprawidłowe ID zgłoszenia.", nameof(id));

        return _unitOfWork.ExecuteInTransactionAsync(() => _ticketRepository.CloseAsync(id));
    }

}