using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    service = "Cinema API Gateway",
    routes = new[]
    {
        "/identity/register",
        "/identity/login",
        "/catalog/get-all",
        "/catalog/search",
        "/orders/create"
    }
}));

await app.UseOcelot();

app.Run();