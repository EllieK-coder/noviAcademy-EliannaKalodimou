using WorldRank.Console.Enums;

namespace WorldRank.Console
{
	public interface IPlayer
	{
		string Name { get; set; }
		int Score { get; set; }
		Dictionary<Currency, Wallet> Wallets { get; set; }
	}
}