using Microsoft.Extensions.DependencyInjection;
using WorldRank.Application.Services;
using WorldRank.Application.Strategies;
using WorldRank.Application.Interfaces;
// Note: register concrete repository implementations explicitly below

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorldRank(this IServiceCollection services)
    {
        // Logging for repositories and services
        services.AddLogging();

        // Repositories (EF Core implementations)
        services.AddScoped<IPlayerRepository, WorldRank.Infrastructures.Data.DBPlayerRepository>();
        services.AddScoped<IWalletRepository, WorldRank.Infrastructures.Data.DBWalletRepository>();

        // Strategies
        services.AddSingleton<IFundsStrategy, AddFundsStrategy>();
        services.AddSingleton<IFundsStrategy, SubtractFundsStrategy>();
        services.AddSingleton<IFundsStrategy, ForceSubtractFundsStrategy>();

        // Application services
        services.AddScoped<PlayerService>();
        services.AddScoped<WalletService>();

        return services;
    }
}
