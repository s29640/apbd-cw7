using Microsoft.Data.SqlClient;
using MiniHelpdesk.Models;

namespace MiniHelpdesk.Infrastructure.Mappers;

internal static class TicketMapper
{
    public static Ticket Map(SqlDataReader reader)
    {
        var idOrdinal = reader.GetOrdinal(nameof(Ticket.Id));
        var titleOrdinal = reader.GetOrdinal(nameof(Ticket.Title));
        var descriptionOrdinal = reader.GetOrdinal(nameof(Ticket.Description));
        var statusOrdinal = reader.GetOrdinal(nameof(Ticket.Status));
        var createdAtOrdinal = reader.GetOrdinal(nameof(Ticket.CreatedAt));

        return new Ticket
        {
            Id = reader.GetInt32(idOrdinal),
            Title = reader.GetString(titleOrdinal),
            Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal),
            Status = reader.GetString(statusOrdinal),
            CreatedAt = reader.GetDateTime(createdAtOrdinal)
        };
    }
}