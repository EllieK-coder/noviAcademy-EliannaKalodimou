using WorldRank;

public interface IWalletRepository
{
    void Add(Wallet wallet, int playerId);
    List<Wallet> GetByPlayerId(int playerId);
}
