using System.Security.Claims;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _appDbContext;

    public UserAccessor(IHttpContextAccessor httpContextAccessor, AppDbContext appDbContext)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
    }

    public async Task<User> GetUserAsync()
    {
        return await _appDbContext.Users.FindAsync(GetUserId()) ??
            throw new InvalidOperationException("User not found in the database.");
    }

    public string GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            throw new InvalidOperationException("User is not authenticated.");
    }

    public async Task<User> GetUserWithPhotosAsync()
    {
        var userId = GetUserId();
        return await _appDbContext.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(x => x.Id.Equals(userId))
                ?? throw new InvalidOperationException("User not found in the database.");
    }

    public bool IsCurrentUser(string userId)
    {
        var currentUserId = GetUserId();
        return currentUserId.Equals(userId, StringComparison.OrdinalIgnoreCase);
    }
}
