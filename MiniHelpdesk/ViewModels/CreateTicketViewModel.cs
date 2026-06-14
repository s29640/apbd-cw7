using System.ComponentModel.DataAnnotations;

namespace MiniHelpdesk.ViewModels;

public class CreateTicketViewModel
{
    [Required(ErrorMessage = "Tytuł jest wymagany.")]
    [StringLength(200, ErrorMessage = "Tytuł może mieć maksymalnie 200 znaków.")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [StringLength(100, ErrorMessage = "Autor może mieć maksymalnie 100 znaków.")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "Treść pierwszego komentarza jest wymagana.")]
    public string FirstCommentContent { get; set; } = string.Empty;
}