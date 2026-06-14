namespace MiniHelpdesk.Services.Models;

public class CreateTicketRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Author { get; set; } = string.Empty;
    public string FirstCommentContent { get; set; } = string.Empty;
}