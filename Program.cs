using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Test_API.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Test_API.Data;
using Microsoft.Extensions.Options;
using Test_API.ExceptionFilters;
using Microsoft.AspNetCore.Mvc.Filters;
using Test_API.Services;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;


var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
var Services = builder.Services;

// Add services to the container
Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});


Services.AddEndpointsApiExplorer();
Services.AddSwaggerGen();



Services.AddSingleton<AppInfoService>();
Services.AddSingleton<RequestAuditService>();

Services.AddTransient<FormatterService>();
Services.AddTransient<DataSeeder>();
Services.AddTransient<BookService>();
Services.AddTransient<AuthorService>();


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
    private readonly RequestDelegate _next;
    private readonly AppInfoService _appInfoService;
    private readonly RequestAuditService _requestAuditService;

    public CustomMiddleware(RequestDelegate next, AppInfoService appInfoService, RequestAuditService requestAuditService)
    {
        _next = next;
        _appInfoService = appInfoService;
        _requestAuditService = requestAuditService;
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
    static int LogWriteHelper(HttpContext http) //for fetching Id from Request 
    {

        var request = http.Request;
        string value = request.Path.Value;
        int id = 0;
        int multiplier = 1;

        for (int i = value.Length - 1; i >= 0; i--)
        {
            char c = value[i];

            if (char.IsDigit(c))
            {
                id += (c - '0') * multiplier;
                multiplier *= 10;
            }
            else
                break;
        }
        return id;
    }

    public async Task Invoke(HttpContext context)
    {
        DateTime startTime = DateTime.Now;

        context.Response.Headers["X-App-Name"] = _appInfoService.GetAppName();
        context.Response.Headers["X-App-Version"] = _appInfoService.GetVersion();

        Console.WriteLine("🔀 Middleware Begins");

        await _next(context);

        var response = context.Response;
        var request = context.Request;

        var host = request.Headers.FirstOrDefault(h => h.Key == "Host").Value;

        int id = LogWriteHelper(context);

        if(request.Method == "GET" && id!=0)
        _requestAuditService.LogWrite("Request", id);

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Time Stamp : {(startTime)} \n");

        string colorcode = StatusColor(request.Method);


        if (Enum.TryParse(colorcode, true, out ConsoleColor parsedColor))
            Console.ForegroundColor = parsedColor;
        else
            Console.ForegroundColor = ConsoleColor.Gray;

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