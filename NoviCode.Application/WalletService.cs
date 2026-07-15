using Microsoft.Extensions.Logging;

namespace NoviCode;

// The service layer owns the in-memory cache (Day 6). Every read is served from the
// cache when possible (cache-aside); every write updates the database first and then
// writes the fresh value straight back into the cache, so cache and DB never disagree.
public class WalletService : IWalletService
{
	private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);

	private readonly IWalletRepository _wallets;
	private readonly ICache _cache;
	private readonly ILogger<WalletService> _logger;

	public WalletService(IWalletRepository wallets, ICache cache, ILogger<WalletService> logger)
	{
		_wallets = wallets;
		_cache = cache;
		_logger = logger;
	}

	// Cache keys: one entry per wallet, plus the two list views.
	private static string WalletKey(Guid id) => $"wallet:{id}";
	private static string PlayerWalletsKey(Guid playerId) => $"wallets:player:{playerId}";
	private const string AllWalletsKey = "wallets:all";

	public async Task<Wallet> CreateWalletAsync(Guid playerId, Currency currency, CancellationToken cancellationToken = default)
	{
		var wallet = new Wallet(playerId, currency);
		await _wallets.AddAsync(wallet, cancellationToken); // DB first
		_logger.LogInformation("Wallet created {WalletId} for player {PlayerId} in {Currency}",
			wallet.Id, playerId, currency);
		Refresh(wallet); // then update the cache
		return wallet;
	}

	public async Task<Wallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		if (_cache.TryGet(WalletKey(id), out Wallet? cached) && cached is not null)
		{
			_logger.LogInformation("Cache HIT  wallet {WalletId}", id);
			return cached;
		}

		_logger.LogInformation("Cache MISS wallet {WalletId} — loading from database", id);
		var wallet = await _wallets.GetByIdAsync(id, cancellationToken);
		if (wallet is not null)
			_cache.Set(WalletKey(id), wallet, Ttl);
		return wallet;
	}

	public async Task<IReadOnlyList<Wallet>> GetByPlayerAsync(Guid playerId, CancellationToken cancellationToken = default)
	{
		if (_cache.TryGet(PlayerWalletsKey(playerId), out IReadOnlyList<Wallet>? cached) && cached is not null)
		{
			_logger.LogInformation("Cache HIT  wallets of player {PlayerId}", playerId);
			return cached;
		}

		_logger.LogInformation("Cache MISS wallets of player {PlayerId} — loading from database", playerId);
		var wallets = await _wallets.GetByPlayerAsync(playerId, cancellationToken);
		_cache.Set(PlayerWalletsKey(playerId), wallets, Ttl);
		return wallets;
	}

	public async Task<IReadOnlyList<Wallet>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		if (_cache.TryGet(AllWalletsKey, out IReadOnlyList<Wallet>? cached) && cached is not null)
		{
			_logger.LogInformation("Cache HIT  all wallets");
			return cached;
		}

		_logger.LogInformation("Cache MISS all wallets — loading from database");
		var wallets = await _wallets.GetAllAsync(cancellationToken);
		_cache.Set(AllWalletsKey, wallets, Ttl);
		return wallets;
	}

	// Day 4: the fund operation is chosen by the injected strategy — no if/switch here.
	public async Task ApplyFundsAsync(Wallet wallet, decimal amount, IFundsStrategy strategy, CancellationToken cancellationToken = default)
	{
		strategy.Execute(wallet, amount);
		await _wallets.SaveChangesAsync(cancellationToken);
		_logger.LogInformation("Applied {Strategy} {Amount} to wallet {WalletId}; new balance {Balance}",
			strategy.GetType().Name, amount, wallet.Id, wallet.Balance);
		Refresh(wallet);
	}

	public async Task<Wallet?> DepositAsync(Guid walletId, decimal amount, CancellationToken cancellationToken = default)
	{
		var wallet = await _wallets.GetByIdAsync(walletId, cancellationToken);
		if (wallet is null)
			return null;

		wallet.Deposit(amount); // may throw WalletBlockedException / InvalidAmountException
		await _wallets.SaveChangesAsync(cancellationToken);
		_logger.LogInformation("Deposited {Amount} to wallet {WalletId}; new balance {Balance}",
			amount, walletId, wallet.Balance);
		Refresh(wallet);
		return wallet;
	}

	public async Task SetBlockedAsync(Wallet wallet, bool blocked, CancellationToken cancellationToken = default)
	{
		if (blocked)
			wallet.Block();
		else
			wallet.Unblock();

		await _wallets.SaveChangesAsync(cancellationToken);
		_logger.LogInformation("Wallet {WalletId} is now {State}", wallet.Id, blocked ? "blocked" : "active");
		Refresh(wallet);
	}

	// Write-through: store the fresh wallet under its own key and drop the list caches
	// (all wallets + this player's wallets) so the next read rebuilds them from the DB.
	private void Refresh(Wallet wallet)
	{
		_cache.Set(WalletKey(wallet.Id), wallet, Ttl);
		_cache.Remove(AllWalletsKey);
		_cache.Remove(PlayerWalletsKey(wallet.PlayerId));
		_logger.LogInformation("Cache write-through wallet {WalletId}; list caches invalidated", wallet.Id);
	}
}
