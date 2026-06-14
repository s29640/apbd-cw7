using Microsoft.AspNetCore.Mvc;
using MiniHelpdesk.Services;
using MiniHelpdesk.Services.Models;
using MiniHelpdesk.ViewModels;

namespace MiniHelpdesk.Controllers;

public class TicketsController : Controller
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tickets = await _ticketService.GetAllAsync();

        return View(tickets);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateTicketViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTicketViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new CreateTicketRequest
        {
            Title = model.Title,
            Description = model.Description,
            Author = model.Author,
            FirstCommentContent = model.FirstCommentContent
        };

        try
        {
            var ticketId = await _ticketService.CreateTicketAsync(request);

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var ticket = await _ticketService.GetByIdAsync(id);

        if (ticket is null)
        {
            return NotFound();
        }

        var comments = await _ticketService.GetCommentsAsync(id);

        var model = new TicketDetailsViewModel
        {
            Ticket = ticket,
            Comments = comments
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id)
    {
        await _ticketService.CloseAsync(id);

        return RedirectToAction(nameof(Details), new { id });
    }
}