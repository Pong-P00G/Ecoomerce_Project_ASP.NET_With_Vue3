using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.Services;
using SmallEcommerceApi.Services.Interfaces;
using System.Text.Json;

namespace SmallEcommerceApi
{
    public partial class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database connection string
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Services registration
            builder.Services.AddScoped<ICouponService, CouponService>();
            builder.Services.AddScoped<IAddressService, AddressService>();
            builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            // Cors
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("https://localhost:7074", "http://localhost:3000", "http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // BUILD THE APP AFTER ALL SERVICES ARE REGISTERED
            var app = builder.Build();

            // Ensure database is created (Development only)
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    try
                    {
                        dbContext.Database.EnsureCreated();
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't crash the app
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while creating the database.");
                    }
                }

                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            // USE CORS MIDDLEWARE (this stays after builder.Build())
            app.UseCors();
            //app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}