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

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.

// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// // Configure JWT authentication
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = builder.Configuration["Jwt:Issuer"],
//         ValidAudience = builder.Configuration["Jwt:Audience"],
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//     };
// });

// // Add services to the container.
// builder.Services.AddControllers();
// builder.Services.AddDbContext<UserContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddDbContext<CarContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
//     app.UseDeveloperExceptionPage();
// }

// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.MapControllers();

// app.Run();

namespace Test_API
{
    public class Test
    {
        public string Func(int k)
        {
            string result="";
            if(k%2==0)
            {
                for(int i=0;i<k/2;i++)
                {
                    result += "adak ";
                }
                result = result.Substring(0, result.Length - 1);
            }
            else
            {
                for(int i=0;i<k/2;i++)
                {
                    result += "adak ";
                }
                result+="anane";
            }
            return result;            
        }
        public static string[] Solution(string str)
        {
            return [];
        }

        public class Program
        {
            public static void Main()
            {
                Test test = new();
                var result = test.Func(8);
                
                Console.WriteLine(result);
            }
        }
    }
}