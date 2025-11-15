using API.Middleware;
using Application.Activities.Queries;
using Application.Activities.Validators;
using Application.Core;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddCors();
            builder.Services.AddControllers();

            builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfiles).Assembly);

            builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();

            builder.Services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();
                x.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            builder.Services.AddTransient<ExceptionMiddleware>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseRouting();
            app.UseCors(policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://localhost:3000", "https://localhost:3000");
            });
            app.UseAuthorization();

            app.MapControllers();

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                await context.Database.MigrateAsync();
                await DbInitializer.SeedDataAsync(context);
            }
            catch (Exception exception)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(exception, "An error occurred during migration");
            }

            await app.RunAsync();
        }
    }
}