

using GameStore.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    public static async Task InitializeDbAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        await dbContext.Database.MigrateAsync();

        var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                                    .CreateLogger("DB Initializer"); 

        logger.LogInformation(5,"The Database has been initialized");
    }

    public static IServiceCollection AddRepositories(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
      var connectionString = configuration.GetConnectionString("GameStoreContext");
      services.AddSqlServer<GameStoreContext>(connectionString)
      .AddScoped<IGamesRepository, EntityFrameworkRepository>();

      return services;
    }
}