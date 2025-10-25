using BookStoreEcommerce.Auth.Handlers;
using BookStoreEcommerce.Auth.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreEcommerce.Auth;

public static class Policies
{
    public const string Self = "Self";
    public const string Admin = "Admin";

    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, SelfAuthorizationHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(Self, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new SelfRequirement());
            })
            .AddPolicy(Admin, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
            });

        return services;
    }
}
