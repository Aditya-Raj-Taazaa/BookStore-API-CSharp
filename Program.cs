﻿using Microsoft.EntityFrameworkCore;
using Test_API.Domains;
using Test_API.Middleware;
using Test_API.Data;
using Test_API.ExceptionFilters;
using Test_API.Services;
using Test_API.DTO;
using Test_API.Repositories;
using Test_API.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container
services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddAutoMapper(typeof(MappingProfile));

services.AddSingleton<IAppInfoService, AppInfoService>();
services.AddSingleton<IRequestAuditServices,RequestAuditService>();

services.AddScoped<IFormatterService,FormatterService>();
services.AddScoped<DataSeeder>();
services.AddScoped<IBookRepository, BookRepository>();
services.AddScoped<IAuthorRepository, AuthorRepository>();
services.AddScoped<IBookService, BookService>();
services.AddScoped<IAuthorService, AuthorService>();

services.AddDbContextPool<BookdbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

void ApplyMigrations(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<BookdbContext>();

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

services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var Services = scope.ServiceProvider;
    var seeder = Services.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

// HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    ApplyMigrations(app);
}
app.UseCors("AllowAll");

app.UseMiddleware<RequestStatusMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();