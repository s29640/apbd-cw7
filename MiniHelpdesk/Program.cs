using MiniHelpdesk.Contracts;
using MiniHelpdesk.Infrastructure.Database;
using MiniHelpdesk.Infrastructure.Repositories;
using MiniHelpdesk.Services;
using MiniHelpdesk.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IRequestLogWriter, FileRequestLogWriter>();
builder.Services.AddScoped<ISqlSession, SqlSession>();
builder.Services.AddScoped<IUnitOfWork, SqlUnitOfWork>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddControllersWithViews(options =>
{
})
.AddMvcOptions(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

var app = builder.Build();

Console.WriteLine($"ENV = {app.Environment.EnvironmentName}");

app.UseHttpsRedirection();

app.MapStaticAssets();

app.UseExceptionHandler("/Home/Error");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseRouting();

app.UseMiddleware<RequestTimingMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

