using System.ComponentModel.DataAnnotations;

namespace MiniHelpdesk.ViewModels;

public class CreateTicketViewModel
{
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [StringLength(100, ErrorMessage = "The author name must not exceed 100 characters.")]
    public string Author { get; set; } = string.Empty;

    public string FirstCommentContent { get; set; } = string.Empty;
}