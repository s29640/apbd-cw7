using Microsoft.Data.SqlClient;
using MiniHelpdesk.Models;

namespace MiniHelpdesk.Infrastructure.Mappers;

internal static class TicketCommentMapper
{
    public static TicketComment Map(SqlDataReader reader)
    {
        var idOrdinal = reader.GetOrdinal(nameof(TicketComment.Id));
        var ticketIdOrdinal = reader.GetOrdinal(nameof(TicketComment.TicketId));
        var authorOrdinal = reader.GetOrdinal(nameof(TicketComment.Author));
        var contentOrdinal = reader.GetOrdinal(nameof(TicketComment.Content));
        var createdAtOrdinal = reader.GetOrdinal(nameof(TicketComment.CreatedAt));

        return new TicketComment
        {
            Id = reader.GetInt32(idOrdinal),
            TicketId = reader.GetInt32(ticketIdOrdinal),
            Author = reader.GetString(authorOrdinal),
            Content = reader.GetString(contentOrdinal),
            CreatedAt = reader.GetDateTime(createdAtOrdinal)
        };
    }
}