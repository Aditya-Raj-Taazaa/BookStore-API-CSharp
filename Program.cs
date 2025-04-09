using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Test_API.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Collections.Generic;
using System.Threading;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Reflection;
using System.Formats.Asn1;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
var Services = builder.Services;
// Add services to the container.

Services.AddControllers();
Services.AddEndpointsApiExplorer();
Services.AddSwaggerGen();

// Configure JWT authentication
Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration["Jwt:Issuer"],
        ValidAudience = Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
    };
});

// Add services to the container.
Services.AddControllers();

Services.AddDbContext<UserContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

Services.AddDbContext<CarContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//namespace Test_API
//{
//    public class Test
//    {
//        // Generic method to describe the input
//        public string DescribeInput<T>(T input)
//        {
//            return $"Input Type: {typeof(T)}, Input Value: {input}";
//        }
//    }

//    public class Program
//    {
//        public static void Main()
//        {
//            Console.WriteLine("Enter a value:");
//            int input = Console.Read(); // Accept input as a string
//            Test test = new();

//            MethodInfo methodInfo = typeof(Test).GetMethod("DescribeInput");
//            MethodInfo genericMethod = methodInfo.MakeGenericMethod(typeof(string));
//            object result = genericMethod.Invoke(test, new object[] { input });

//            // var result = test.DescribeInput<int>(input);
//            var result2 = test.DescribeInput<string>(input.ToString());

//            // Print the result
//            Console.WriteLine(result + "-"+ result2);
//        }
//    }
//}