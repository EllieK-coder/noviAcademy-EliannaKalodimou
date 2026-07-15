using Microsoft.Extensions.Logging;

namespace NoviCode;

// Player business logic — creating a player enforces the domain invariants
// (non-empty name, non-negative score). Like WalletService, it owns its cache:
// reads are cache-aside, and creating a player writes through to the cache.
public class PlayerService : IPlayerService
{
	private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);

	private readonly IPlayerRepository _players;
	private readonly ICache _cache;
	private readonly ILogger<PlayerService> _logger;

	public PlayerService(IPlayerRepository players, ICache cache, ILogger<PlayerService> logger)
	{
		_players = players;
		_cache = cache;
		_logger = logger;
	}

	private static string PlayerKey(Guid id) => $"player:{id}";
	private const string AllPlayersKey = "players:all";

	public async Task<Player> CreateAsync(string name, int score, CancellationToken cancellationToken = default)
	{
		var player = new Player(name); // throws on empty name
		player.UpdateScore(score);     // throws on negative score

		await _players.AddAsync(player, cancellationToken); // DB first
		_logger.LogInformation("Player created {PlayerId} {Name} (score {Score})", player.Id, name, score);

		_cache.Set(PlayerKey(player.Id), player, Ttl); // write-through
		_cache.Remove(AllPlayersKey);                  // invalidate the list
		_logger.LogInformation("Cache write-through player {PlayerId}; list cache invalidated", player.Id);
		return player;
	}

	public async Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		if (_cache.TryGet(PlayerKey(id), out Player? cached) && cached is not null)
		{
			_logger.LogInformation("Cache HIT  player {PlayerId}", id);
			return cached;
		}

		_logger.LogInformation("Cache MISS player {PlayerId} — loading from database", id);
		var player = await _players.GetByIdAsync(id, cancellationToken);
		if (player is not null)
			_cache.Set(PlayerKey(id), player, Ttl);
		return player;
	}

	public async Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		if (_cache.TryGet(AllPlayersKey, out IReadOnlyList<Player>? cached) && cached is not null)
		{
			_logger.LogInformation("Cache HIT  all players");
			return cached;
		}

		_logger.LogInformation("Cache MISS all players — loading from database");
		var players = await _players.GetAllAsync(cancellationToken);
		_cache.Set(AllPlayersKey, players, Ttl);
		return players;
	}
}
