using Microsoft.Extensions.Logging;
using NoviCode.Commands;

namespace NoviCode.Decorators;

public sealed class CachingQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>, ICacheableQuery
{
    private readonly IQueryHandler<TQuery, TResult> _inner;
    private readonly ICache _cache;
    private readonly ILogger<CachingQueryHandlerDecorator<TQuery, TResult>> _logger;

    public CachingQueryHandlerDecorator(
        IQueryHandler<TQuery, TResult> inner,
        ICache cache,
        ILogger<CachingQueryHandlerDecorator<TQuery, TResult>> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default)
    {
        if (_cache.TryGet(query.CacheKey, out TResult? cached) && cached is not null)
        {
            _logger.LogInformation("Cache HIT  {Key}", query.CacheKey);
            return cached;
        }

        _logger.LogInformation("Cache MISS {Key} — loading from database", query.CacheKey);
        var result = await _inner.HandleAsync(query, ct);
        if (result is not null)
            _cache.Set(query.CacheKey, result, query.CacheTtl);
        return result;
    }
}

public sealed class WalletCacheWriteThroughDecorator<TCommand> : ICommandHandler<TCommand, Wallet?>
    where TCommand : ICommand<Wallet?>
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);
    private readonly ICommandHandler<TCommand, Wallet?> _inner;
    private readonly ICache _cache;
    private readonly ILogger<WalletCacheWriteThroughDecorator<TCommand>> _logger;

    public WalletCacheWriteThroughDecorator(
        ICommandHandler<TCommand, Wallet?> inner, ICache cache,
        ILogger<WalletCacheWriteThroughDecorator<TCommand>> logger)
    {
        _inner = inner; _cache = cache; _logger = logger;
    }

    public async Task<Wallet?> HandleAsync(TCommand command, CancellationToken ct = default)
    {
        var wallet = await _inner.HandleAsync(command, ct);
        if (wallet is not null)
        {
            _cache.Set(CacheKeys.Wallet(wallet.Id), wallet, Ttl);
            _cache.Remove(CacheKeys.AllWallets);
            _cache.Remove(CacheKeys.PlayerWallets(wallet.PlayerId));
            _logger.LogInformation("Cache write-through wallet {WalletId}; balance {Balance}", wallet.Id, wallet.Balance);
        }
        return wallet;
    }
}

public sealed class PlayerCacheWriteThroughDecorator<TCommand> : ICommandHandler<TCommand, Player?>
    where TCommand : ICommand<Player?>
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);
    private readonly ICommandHandler<TCommand, Player?> _inner;
    private readonly ICache _cache;
    private readonly ILogger<PlayerCacheWriteThroughDecorator<TCommand>> _logger;

    public PlayerCacheWriteThroughDecorator(
        ICommandHandler<TCommand, Player?> inner, ICache cache,
        ILogger<PlayerCacheWriteThroughDecorator<TCommand>> logger)
    {
        _inner = inner; _cache = cache; _logger = logger;
    }

    public async Task<Player?> HandleAsync(TCommand command, CancellationToken ct = default)
    {
        var player = await _inner.HandleAsync(command, ct);
        if (player is not null)
        {
            _cache.Set(CacheKeys.Player(player.Id), player, Ttl);
            _cache.Remove(CacheKeys.AllPlayers);
            _logger.LogInformation("Cache write-through player {PlayerId}", player.Id);
        }
        return player;
    }
}