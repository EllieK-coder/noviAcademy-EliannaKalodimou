using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using WorldRank.Application.Services;
using WorldRank.Infrastructures.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        var logger = LogManager.GetCurrentClassLogger();

        // Composition root: register every layer's services, then build the container.
        var services = new ServiceCollection();
        services.AddWorldRank();

        // Register the EF Core DbContext on the same collection.
        services.AddDbContext<WorldRankDbContext>(options =>
            options.UseSqlServer("Server=localhost;Database=WorldRank;Integrated Security=true;TrustServerCertificate=true"));

        using var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var playerService = scope.ServiceProvider.GetRequiredService<PlayerService>();
        var walletService = scope.ServiceProvider.GetRequiredService<WalletService>();

        logger.Info("Application started.");

        while (true)
        {
            Console.WriteLine("\n=== WorldRank Player Registry ===");
            Console.WriteLine("--- Players ---");
            Console.WriteLine("1. Add player");
            Console.WriteLine("2. List all players");
            Console.WriteLine("3. List players grouped by score");
            Console.WriteLine("4. Find player by name");
            Console.WriteLine("5. Find player by id");
            Console.WriteLine("6. Delete player");
            Console.WriteLine("--- Wallets ---");
            Console.WriteLine("7. Add wallet to player");
            Console.WriteLine("8. Show player wallets");
            Console.WriteLine("9. Deposit to wallet");
            Console.WriteLine("10. Withdraw from wallet");
            Console.WriteLine("11. Block wallet");
            Console.WriteLine("12. Unblock wallet");
            Console.WriteLine("13. Update wallet balance");
            Console.WriteLine("14. Apply funds operation (strategy)");
            Console.WriteLine("0. Exit");
            Console.Write("> ");

            Action? action = Console.ReadLine() switch
            {
                "1" => playerService.AddPlayer,
                "2" => playerService.ListPlayers,
                "3" => playerService.ListPlayersByScore,
                "4" => playerService.FindPlayerByName,
                "5" => playerService.FindPlayerById,
                "6" => playerService.DeletePlayer,
                "7" => walletService.AddWalletToPlayer,
                "8" => walletService.GetWalletsOfPlayer,
                "9" => walletService.DepositToWallet,
                "10" => walletService.WithdrawFromWallet,
                "11" => walletService.BlockWallet,
                "12" => walletService.UnblockWallet,
                "13" => walletService.UpdateWalletBalance,
                "14" => walletService.ApplyFundsStrategy,
                "0" => null,
                _ => () => Console.WriteLine("Unknown option.")
            };

            if (action is null)
            {
                logger.Info("Application exiting.");
                LogManager.Shutdown(); // flush file writes before exit
                return; // "0" selected — exit
            }

            try
            {
                action();
            }
            catch (Exception ex)
            {
                // Safety net: log any exception the specific handlers did not catch, and keep the app running.
                logger.Error(ex, "Unexpected error while handling a menu action");
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}