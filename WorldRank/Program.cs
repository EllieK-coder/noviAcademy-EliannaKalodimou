using NLog;
using WorldRank;

var logger = LogManager.GetLogger("Program");

var players = new List<Player>();
var walletRepo = new InMemoryWalletRepository();

while (true)
{
    Console.WriteLine("\n=== WorldRank Player Registry ===");
    Console.WriteLine("1. Add player");
    Console.WriteLine("2. List all players");
    Console.WriteLine("3. Find player by name");
    Console.WriteLine("4. Withdraw from a player's wallet");
    Console.WriteLine("0. Exit");
    Console.Write("> ");

    Action? action = Console.ReadLine() switch
    {
        "1" => AddPlayer,
        "2" => ListPlayers,
        "3" => FindPlayer,
        "4" => WithdrawFromPlayer,
        "0" => null,
        _ => () => Console.WriteLine("Unknown option.")
    };

    if (action is null)
    {
        logger.Info("Application exiting.");
        LogManager.Shutdown();
        return;
    }

    action();
}

void AddPlayer()
{
    Console.Write("Name: ");
    var name = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Name cannot be empty.");
        return;
    }

    Console.Write("Score: ");
    var scoreInput = Console.ReadLine();
    if (!int.TryParse(scoreInput, out var score))
    {
        Console.WriteLine("Score must be a whole number.");
        return;
    }

    var player = new Player(name);
    player.UpdateScore(score);
    players.Add(player);

    // Store wallets in the repository — this is what triggers the bug.
    var w1 = new Wallet(1000m, Wallet.Currency.EUR, false);
    var w2 = new Wallet(500m, Wallet.Currency.USD, false);

    walletRepo.Add(w1, player.Id);   // first call — succeeds
    walletRepo.Add(w2, player.Id);   // second call, same Id — BUG fires here

    Console.WriteLine("Player added successfully.");
}

void ListPlayers()
{
    if (players.Count == 0)
    {
        Console.WriteLine("No players registered.");
        return;
    }

    foreach (var p in players)
        Console.WriteLine(p);
}

void FindPlayer()
{
    Console.Write("Search by name: ");
    var term = Console.ReadLine() ?? string.Empty;

    var player = players
        .FirstOrDefault(p => p.Name.Equals(term, StringComparison.OrdinalIgnoreCase));

    if (player is null)
    {
        Console.WriteLine("No player found.");
        return;
    }

    Console.WriteLine(player);
}

void WithdrawFromPlayer()
{
    Console.Write("Player name: ");
    var term = Console.ReadLine() ?? string.Empty;

    var player = players
        .FirstOrDefault(p => p.Name.Equals(term, StringComparison.OrdinalIgnoreCase));

    if (player is null)
    {
        Console.WriteLine("No player found.");
        return;
    }

    Console.Write("Amount to withdraw: ");
    if (!decimal.TryParse(Console.ReadLine(), out var amount))
    {
        Console.WriteLine("Amount must be a number.");
        return;
    }

    if (!player.Wallets.TryGetValue(Player.Currency.EUR, out var wallet))
    {
        Console.WriteLine("Player has no EUR wallet.");
        return;
    }

    try
    {
        wallet.Withdraw(amount);
        Console.WriteLine($"Withdrawal successful. New balance: {wallet.Balance}");
    }
    catch (InsufficientFundsException ex)
    {
        Console.WriteLine("Not enough funds.");
        logger.Error(ex, "Withdrawal failed due to insufficient funds.");
    }
    catch (WalletException ex)
    {
        Console.WriteLine("Wallet error.");
        logger.Error(ex, "Withdrawal failed due to a wallet error.");
    }
}