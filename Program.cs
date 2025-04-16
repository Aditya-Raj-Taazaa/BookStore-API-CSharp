using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Test_API.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Test_API.Data;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
var Services = builder.Services;

// Add services to the container
Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

Services.AddEndpointsApiExplorer();
Services.AddSwaggerGen();
Services.AddControllers();

builder.Services.AddTransient<DataSeeder>();



Services.AddDbContextPool<BookdbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

void ApplyMigrations(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<BookdbContext>();

        // Check and apply pending migrations
        var pendingMigrations = dbContext.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            Console.WriteLine("Applying pending migrations...");
            dbContext.Database.Migrate();
            Console.WriteLine("Migrations applied successfully.");
        }
        else
        {
            Console.WriteLine("No pending migrations found.");
        }
    }
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    ApplyMigrations(app);
}

app.UseMiddleware<CustomMiddleware>(); 

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



public class CustomMiddleware
{
    public readonly RequestDelegate _next;

    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    static string StatusColor(string method)
    {
        if (method == "GET")
            return "Green";
        else if (method == "PUT")
            return "Blue";
        else if (method == "DELETE")
            return "Red";
        else
            return "Yellow";
    }

    public async Task Invoke(HttpContext context)
    {
        DateTime startTime = DateTime.Now;
        Console.WriteLine("🔀 Middleware Begins");
        await _next(context);

        DateTime endTime = DateTime.Now;
        var response = context.Response;
        var request = context.Request;

        var host = request.Headers.FirstOrDefault(h => h.Key == "Host").Value;

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Time taken by the process: {(endTime - startTime).TotalMilliseconds} ms \n");

        string colorcode = StatusColor(request.Method);


        if (Enum.TryParse(colorcode, true, out ConsoleColor parsedColor))
            Console.ForegroundColor = parsedColor;
        else
            Console.ForegroundColor = ConsoleColor.Gray; // Fallback color

        Console.WriteLine($"Method: {request.Method}");
        Console.WriteLine($"Endpoint: {host}" + $"{request.Path.Value}");

        Console.WriteLine($"Response Status Code: {response.StatusCode}");

        Console.WriteLine($"Content Type: {response.ContentType}");
        Console.WriteLine($"Request Body: {await new StreamReader(request.Body).ReadToEndAsync()}");
        Console.WriteLine($"Response Body : {response.Body.ToString()}");
        Console.WriteLine($"Request Cookies: {string.Join(", ", request.Cookies.Select(c => $"{c.Key}={c.Value}"))}");
        Console.WriteLine($"Request Headers: {string.Join(", ", request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
        Console.ResetColor();
    }
}