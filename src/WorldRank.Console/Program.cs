using WorldRank.Console;
using WorldRank.Console.Enums;

var players = new List<Player>();
var nextId = 1;
//Create an instance of our WalletRepositoy passing the player list
IWalletRepository walletRepository = new InMemoryWalletRepository(players);
IPlayerRepository playerRepository = new InMemoryPlayerRepository(players);

while (true)
{
	Console.WriteLine("\n=== WorldRank Player Registry ===");
	Console.WriteLine("1. Add player");
	Console.WriteLine("2. List all players");
	Console.WriteLine("3. Find player by name");
	Console.WriteLine("4. Add Wallet to player");
	Console.WriteLine("5. Get Player Wallets");
	Console.WriteLine("0. Exit");
	Console.Write("> ");

	Action? action = Console.ReadLine() switch
	{
		"1" => AddPlayer,
		"2" => ListPlayers,
		"3" => FindPlayer,
		//New functionality
		"4" => AddWalletToPlayer,
		"5" => GetWalletOfPlayer,
		"0" => null,
		_ => () => Console.WriteLine("Unknown option.")
	};

	if (action is null)
		return; // "0" selected — exit

	action();
}

#region Player Methods

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

	var player = new Player(nextId++, name);
	player.UpdateScore(score);
	playerRepository.AddPlayer(player);
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

void FindPlayerById()
{
	Console.Write("Search by Id: ");
	var term = Console.ReadLine() ?? string.Empty;

	if (!int.TryParse(term, out var id))
	{
		Console.WriteLine("Player id is not a number");
	}

	var player = playerRepository.FindPlayer(id);

	if (player is null)
	{
		Console.WriteLine("No player found.");
		return;
	}

	Console.WriteLine(player);
}

void SearchPlayer()
{
	Console.Write("Give player id: ");
	var id = Console.ReadLine();
	int.TryParse(id, out var playerId);
	{
		walletRepository.Add(new Wallet(10, Currency.EUR, false), playerId);
	}
	Console.Write("Id not a number");
}

#endregion Player Methods

#region Wallet Methods

void AddWalletToPlayer()
{
	Console.Write("Give player id: ");
	var id = Console.ReadLine();
	Console.Write("Give Currency: 0 - NONE |  1 - EUR | 2 - USD\n");
	var currency = Console.ReadLine();

	Currency cur = Currency.NONE;

	switch (currency)
	{
		case "0":
		default:
			cur = Currency.NONE;
			break;

		case "1":
			cur =
			Currency.EUR;
			break;
		case "2":
			cur =
			Currency.USD;
			break;
	}

	int.TryParse(id, out var playerId);
	{
		walletRepository.Add(new Wallet(10, cur, false), playerId);
	}
}

void GetWalletOfPlayer()
{
	Console.Write("Give player id: ");
	var id = Console.ReadLine();

	if (int.TryParse(id, out var playerId))
	{
		var wallets = walletRepository.GetByPlayer(playerId);

		foreach (var wallet in wallets)
		{
			Console.WriteLine($"Wallet Number {wallets.IndexOf(wallet)} {wallet.ToString()}");
		}
	}
	else
	{
		Console.Write("Id not a number");
	}
}

#endregion Wallet Methods