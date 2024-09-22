namespace GameStore.Api.Authorization;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddGameStoreAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.ReadAccess, build =>
            {
                build.RequireClaim("scope", "games:read");
            });

            options.AddPolicy(Policies.WriteAccess, build =>
            {
                build.RequireClaim("scope", "games:write");
            });
        });

        return services;
    }
}
