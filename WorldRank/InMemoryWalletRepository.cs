using NLog;

namespace WorldRank;

public class InMemoryWalletRepository : IWalletRepository
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly Dictionary<int, List<Wallet>> _walletsByPlayer = new();

    public void Add(Wallet wallet, int playerId)
    {
        // If the player already has wallets stored, append to the list.
        // Otherwise create a new list and add it to the dictionary.
        if (_walletsByPlayer.TryGetValue(playerId, out var existing))
        {
            existing.Add(wallet);
        }
        else
        {
            _walletsByPlayer.Add(playerId, new List<Wallet> { wallet });
        }

        _logger.Info("Added wallet for player {PlayerId}.", playerId);
    }

    public List<Wallet> GetByPlayerId(int playerId)
    {
        if (!_walletsByPlayer.TryGetValue(playerId, out var wallets))
        {
            _logger.Warn("No wallets found for player {PlayerId}.", playerId);
            return new List<Wallet>();
        }

        return wallets;
    }
}