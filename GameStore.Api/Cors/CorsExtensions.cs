namespace GameStore.Api.Cors;

public static class CorsExtensions
{
    private const string allowedOriginSetting = "AllowedOrigins";

    public static IServiceCollection AddGameStoreCors(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
       return services.AddCors(options =>
        {
            options.AddDefaultPolicy(corsBuilder =>
            {
                var allowedOrigins = configuration[allowedOriginSetting]
                    ?? throw new InvalidOperationException($"{allowedOriginSetting} is not configured in appsettings.json");
                corsBuilder.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("X-Pagination");
            });
        });
    }
}