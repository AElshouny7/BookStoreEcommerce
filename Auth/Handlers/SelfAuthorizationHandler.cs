using System.Security.Claims;
using BookStoreEcommerce.Auth.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStoreEcommerce.Auth.Handlers;


public sealed class SelfAuthorizationHandler : AuthorizationHandler<SelfRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SelfRequirement requirement)
    {
        var claimUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(claimUserId))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var httpContext =
        (context.Resource as AuthorizationFilterContext)?.HttpContext
            ?? (context.Resource as DefaultHttpContext);

        if (httpContext == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var routeIdString = httpContext.GetRouteData()?.Values["id"]?.ToString();
        if (!int.TryParse(claimUserId, out var tokenUserId)
                && int.TryParse(routeIdString, out var routeUserId))
            if (tokenUserId == routeUserId)
                context.Succeed(requirement);

        return Task.CompletedTask;


    }
}