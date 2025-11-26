using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext : IdentityDbContext<User>
{
    public required DbSet<Activity> Activities { get; set; }

    public AppDbContext(DbContextOptions options) : base(options) { }
    
}
