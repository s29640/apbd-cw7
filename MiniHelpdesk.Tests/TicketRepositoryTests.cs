using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MiniHelpdesk.Infrastructure.Repositories;
using MiniHelpdesk.Models;

namespace MiniHelpdesk.Tests.RepositoryTests;

public class TicketRepositoryTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllAsync_Should_Return_More_Than_Zero_Ticket()
    {
        var repository = new TicketRepository(TestConfiguration.Create());

        var tickets = await repository.GetAllAsync();

        Assert.True(tickets.Count > 0);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByIdAsync_Should_Return_Ticket_With_Id_1()
    {
        var repository = new TicketRepository(TestConfiguration.Create());

        var ticket = await repository.GetByIdAsync(1);

        Assert.NotNull(ticket);
        Assert.Equal(1, ticket!.Id);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetByIdAsync_Should_Return_Seeded_Ticket()
    {
        var repository = new TicketRepository(TestConfiguration.Create());

        var ticket = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(ticket);

        Assert.Equal(1, ticket!.Id);
        Assert.Equal("Nie działa drukarka", ticket.Title);
        Assert.Equal("Open", ticket.Status);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetCommentsAsync_Should_Return_At_Least_One_Comment()
    {
        var repository = new TicketRepository(TestConfiguration.Create());

        var comments = await repository.GetCommentsAsync(1);

        Assert.NotNull(comments);
        Assert.NotEmpty(comments);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetCommentsAsync_Should_Contain_Seeded_Comment()
    {
        var repository = new TicketRepository(TestConfiguration.Create());

        var comments = await repository.GetCommentsAsync(1);

        Assert.NotEmpty(comments);

        var comment = comments.SingleOrDefault(c =>
            c.Author == "Jan Kowalski" &&
            c.Content == "Drukarka pokazuje błąd Paper Jam.");

        Assert.NotNull(comment);
        Assert.Equal(1, comment.TicketId);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task InsertCommentAsync_Should_Insert_Comment_And_Then_Delete_It()
    {
        var repository = new TicketRepository(TestConfiguration.Create());

        var comment = new TicketComment
        {
            TicketId = 1,
            Author = "Integration Test",
            Content = $"Test comment {Guid.NewGuid()}",
            CreatedAt = DateTime.UtcNow
        };

        await using var connection = new SqlConnection(TestConfiguration.Create().GetConnectionString("DefaultConnection"));

        await connection.OpenAsync();

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        int commentId = 0;

        try
        {
            commentId = await repository.InsertCommentAsync(comment, connection, transaction);

            await transaction.CommitAsync();

            // Sprawdzenie, czy komentarz istnieje
            var comments = await repository.GetCommentsAsync(1);

            var insertedComment = comments.SingleOrDefault(c =>
                c.Id == commentId &&
                c.Author == comment.Author &&
                c.Content == comment.Content);

            Assert.NotNull(insertedComment);
            Assert.Equal(1, insertedComment!.TicketId);
        }
        finally
        {
            if (commentId > 0)
            {
                var deletedRows = await repository.DeleteCommentAsync(commentId);
                Assert.Equal(1, deletedRows);
            }
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task InsertTicketAsync_Should_Insert_Ticket_And_Then_Delete_It()
    {
        var repository = new TicketRepository(TestConfiguration.Create());

        var ticket = new Ticket
        {
            Title = $"Integration Test {Guid.NewGuid()}",
            Description = "Created by integration test",
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        await using var connection = new SqlConnection(TestConfiguration.Create().GetConnectionString("DefaultConnection"));

        await connection.OpenAsync();

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        int ticketId = 0;

        try
        {
            ticketId = await repository.InsertTicketAsync(ticket, connection, transaction);

            await transaction.CommitAsync();

            var insertedTicket = await repository.GetByIdAsync(ticketId);

            Assert.NotNull(insertedTicket);
            Assert.Equal(ticketId, insertedTicket!.Id);
            Assert.Equal(ticket.Title, insertedTicket.Title);
            Assert.Equal(ticket.Description, insertedTicket.Description);
            Assert.Equal(ticket.Status, insertedTicket.Status);
        }
        finally
        {
            if (ticketId > 0)
            {
                var deletedRows = await repository.DeleteTicketAsync(ticketId);

                Assert.Equal(1, deletedRows);
            }
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CloseAsync_Should_Close_Newly_Created_Ticket()
    {
        var repository = new TicketRepository(TestConfiguration.Create());

        var ticket = new Ticket
        {
            Title = $"Integration Test {Guid.NewGuid()}",
            Description = "Created by integration test",
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        await using var connection = new SqlConnection(TestConfiguration.Create().GetConnectionString("DefaultConnection"));

        await connection.OpenAsync();

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        int ticketId = 0;

        try
        {
            ticketId = await repository.InsertTicketAsync(
                ticket,
                connection,
                transaction);

            await transaction.CommitAsync();

            await repository.CloseAsync(ticketId);

            var updatedTicket = await repository.GetByIdAsync(ticketId);

            Assert.NotNull(updatedTicket);
            Assert.Equal(ticketId, updatedTicket!.Id);
            Assert.Equal(TicketStatus.Closed, updatedTicket.Status);
        }
        finally
        {
            if (ticketId > 0)
            {
                var deletedRows = await repository.DeleteTicketAsync(ticketId);

                Assert.Equal(1, deletedRows);
            }
        }
    }

}