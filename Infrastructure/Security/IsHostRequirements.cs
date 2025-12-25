using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

public class IsHostRequirement : IAuthorizationRequirement { }

public class IsHostRequirementHandler(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
    : AuthorizationHandler<IsHostRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
    {
        var userId = context.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return;
        
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null) return;

        if (httpContext.GetRouteValue("id") is not string activityId) return;
        
        var attendee = await appDbContext.ActivityAttendees
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ActivityId == activityId && x.UserId == userId);

        if (attendee is null) return;

        if (attendee.IsHost) context.Succeed(requirement);
    }
}