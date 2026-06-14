using System.Diagnostics;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using MiniHelpdesk.Models;

namespace MiniHelpdesk.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _environment;

    public HomeController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> License()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "LICENSE");

        var text = System.IO.File.Exists(path)
            ? await System.IO.File.ReadAllTextAsync(path)
            : "License file not found.";

        return View(model: text);
    }

    public async Task<IActionResult> Readme()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "README.md");

        var markdown = await System.IO.File.ReadAllTextAsync(path);

        var html = Markdown.ToHtml(markdown);

        return View(model: html);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}