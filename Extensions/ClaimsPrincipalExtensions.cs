namespace BookStoreEcommerce.Extensions;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static int? TryGetUserId(this ClaimsPrincipal user)
    => int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;
}