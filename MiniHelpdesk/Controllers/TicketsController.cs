using Microsoft.AspNetCore.Mvc;
using MiniHelpdesk.Services;
using MiniHelpdesk.Services.Models;

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
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTicketRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var ticketId = await _ticketService.CreateTicketAsync(request);

            return RedirectToAction(nameof(Details), new { id = ticketId });
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);

            return View(request);
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

        ViewBag.Comments = comments;

        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id)
    {
        await _ticketService.CloseAsync(id);

        return RedirectToAction(nameof(Details), new { id });
    }
}