using API.Controllers;
using API.Middleware;
using API.SignalR;
using Application.Activities.Queries;
using Application.Activities.Validators;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using Infrastructure.Email;
using Infrastructure.Photos;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Resend;

namespace API
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddCors();
            builder.Services.AddSignalR();
            builder.Services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfiles).Assembly);

            builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();

            builder.Services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();
                x.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            builder.Services.AddHttpClient<ResendClient>();
            builder.Services.Configure<ResendClientOptions>(builder.Configuration.GetSection(nameof(ResendClientOptions)));
            builder.Services.AddTransient<IResend, ResendClient>();
            builder.Services.AddTransient<IEmailSender<User>, EmailSender>();

            builder.Services.AddTransient<ExceptionMiddleware>();
            builder.Services
                .AddIdentityApiEndpoints<User>(opt =>
                {
                    opt.User.RequireUniqueEmail = true;
                    opt.SignIn.RequireConfirmedEmail = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                ;

            builder.Services.AddScoped<IUserAccessor, UserAccessor>();
            builder.Services.AddScoped<IPhotoService, PhotoService>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("IsActivityHost", policy =>
                {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            builder.Services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:3000", "https://localhost:3000");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            // app.UseDefaultFiles();
            // app.UseStaticFiles();

            app.MapControllers();
            app.MapGroup("api").MapIdentityApi<User>();
            app.MapHub<CommentHub>("/comments");
            app.MapFallbackToController(
                nameof(FallbackController.Index),
                nameof(FallbackController).Replace("Controller", "", StringComparison.OrdinalIgnoreCase)
            );

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();

                if (!await context.Database.CanConnectAsync())
                    await context.Database.MigrateAsync();

                await DbInitializer.SeedDataAsync(context, userManager);
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