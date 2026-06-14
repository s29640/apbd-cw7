using Microsoft.Data.SqlClient;
using MiniHelpdesk.Contracts;
using MiniHelpdesk.Infrastructure.Database;
using MiniHelpdesk.Infrastructure.Mappers;
using MiniHelpdesk.Models;
using System.Data;

namespace MiniHelpdesk.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly ISqlSession _session;

    public TicketRepository(ISqlSession session)
    {
        _session = session;
    }

    public async Task<IReadOnlyList<Ticket>> GetAllAsync()
    {
        var result = new List<Ticket>();

        await using var command = new SqlCommand("""
            SELECT Id, Title, Description, Status, CreatedAt
            FROM dbo.Tickets
            ORDER BY CreatedAt DESC;
            """, _session.Connection, _session.Transaction);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(TicketMapper.Map(reader));
        }

        return result;
    }

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        await using var command = new SqlCommand("""
            SELECT Id, Title, Description, Status, CreatedAt
            FROM dbo.Tickets
            WHERE Id = @Id;
            """, _session.Connection, _session.Transaction);

        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync()  ? TicketMapper.Map(reader) : null;
    }

    public async Task<IReadOnlyList<TicketComment>> GetCommentsAsync(int ticketId)
    {
        var result = new List<TicketComment>();

        await using var command = new SqlCommand("""
            SELECT Id, TicketId, Author, Content, CreatedAt
            FROM dbo.TicketComments
            WHERE TicketId = @TicketId
            ORDER BY CreatedAt ASC;
            """, _session.Connection, _session.Transaction);

        command.Parameters.Add("@TicketId", SqlDbType.Int).Value = ticketId;

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(TicketCommentMapper.Map(reader));
        }

        return result;
    }

    public async Task<int> DeleteTicketAsync(int id)
    {
        await using var command = new SqlCommand("""
        DELETE FROM dbo.Tickets
        WHERE Id = @Id;
        """, _session.Connection, _session.Transaction);

        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        return await command.ExecuteNonQueryAsync();
    }

    public async Task<int> InsertTicketAsync(Ticket ticket)
    {
        await using var command = new SqlCommand("""
            INSERT INTO dbo.Tickets (Title, Description, Status, CreatedAt)
            OUTPUT INSERTED.Id
            VALUES (@Title, @Description, @Status, @CreatedAt);
            """, _session.Connection, _session.Transaction);

        command.Parameters.Add("@Title", SqlDbType.NVarChar, 200).Value = ticket.Title;

        command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = ticket.Description ?? (object)DBNull.Value;

        command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value =  ticket.Status;

        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value =  ticket.CreatedAt;

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<int> InsertCommentAsync(TicketComment comment)
    {
        await using var command = new SqlCommand("""
        INSERT INTO dbo.TicketComments(TicketId, Author, Content, CreatedAt)
        OUTPUT INSERTED.Id
        VALUES (@TicketId, @Author, @Content, @CreatedAt);
        """, _session.Connection, _session.Transaction);

        command.Parameters.Add("@TicketId", SqlDbType.Int).Value = comment.TicketId;

        command.Parameters.Add("@Author", SqlDbType.NVarChar, 100).Value = comment.Author;

        command.Parameters.Add("@Content", SqlDbType.NVarChar).Value = comment.Content;

        command.Parameters.Add("@CreatedAt", SqlDbType.DateTime2).Value = comment.CreatedAt;

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<int> DeleteCommentAsync(int id)
    {
        await using var command = new SqlCommand("""
        DELETE FROM dbo.TicketComments
        WHERE Id = @Id;
        """, _session.Connection, _session.Transaction);

        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        return await command.ExecuteNonQueryAsync();
    }

    public async Task CloseAsync(int id)
    {
        await using var command = new SqlCommand("""
            UPDATE dbo.Tickets
            SET Status = @Status
            WHERE Id = @Id;
            """, _session.Connection, _session.Transaction);

        command.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value =  TicketStatus.Closed;

        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await command.ExecuteNonQueryAsync();
    }

}