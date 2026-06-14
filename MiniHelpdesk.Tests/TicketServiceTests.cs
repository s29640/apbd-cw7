using MiniHelpdesk.Models;
using MiniHelpdesk.Services;
using MiniHelpdesk.Services.Models;
using MiniHelpdesk.Tests.InMemory;

namespace MiniHelpdesk.Tests.Services;

public class TicketServiceTests
{
    private static TicketService CreateService(InMemoryTicketRepository repository)
    {
        return new TicketService(repository, new InMemoryUnitOfWork());
    }

    [Fact]
    public async Task CreateTicketAsync_ValidRequest_CreatesTicket()
    {
        var repository = new InMemoryTicketRepository();
        var service = CreateService(repository);

        var request = new CreateTicketRequest
        {
            Title = "Problem z logowaniem",
            Description = "Nie mogę zalogować się do systemu.",
            Author = "Robert",
            FirstCommentContent = "Problem występuje od rana."
        };

        var ticketId = await service.CreateTicketAsync(request);

        var ticket = await repository.GetByIdAsync(ticketId);
        var comments = await repository.GetCommentsAsync(ticketId);

        Assert.NotNull(ticket);
        Assert.Equal("Problem z logowaniem", ticket.Title);
        Assert.Equal(TicketStatus.Open, ticket.Status);

        Assert.Single(comments);
        Assert.Equal("Problem występuje od rana.", comments[0].Content);
    }

    [Fact]
    public async Task CreateTicketAsync_EmptyTitle_ThrowsValidationError()
    {
        var repository = new InMemoryTicketRepository();
        var service = CreateService(repository);

        var request = new CreateTicketRequest
        {
            Title = "",
            Description = "Opis",
            Author = "Robert",
            FirstCommentContent = "Pierwszy komentarz"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateTicketAsync(request));
    }

    [Fact]
    public async Task CloseAsync_ExistingTicket_ChangesStatusToClosed()
    {
        var repository = new InMemoryTicketRepository();
        var service = CreateService(repository);

        var ticketId = await service.CreateTicketAsync(new CreateTicketRequest
        {
            Title = "Problem z drukarką",
            Description = "Drukarka nie drukuje.",
            Author = "Robert",
            FirstCommentContent = "Sprawdzone połączenie USB."
        });

        await service.CloseAsync(ticketId);

        var ticket = await repository.GetByIdAsync(ticketId);

        Assert.NotNull(ticket);
        Assert.Equal(TicketStatus.Closed, ticket.Status);
    }
}