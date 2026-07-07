using WorldRank.Console.Enums;

namespace WorldRank.Console;

public class Player : IPlayer
{
	public int Id { get; private set; }
	public string Name { get; set; }
	public int Score { get; set; }
	public Dictionary<Currency, Wallet> Wallets { get; set; } = new Dictionary<Currency, Wallet>();

	public Player(int id, string name)
	{
		Name = name;
		Id = id;
	}

	public override string ToString() => $"[{Id}] {Name} - Score: {Score}";

	public void UpdateScore(int newScore)
	{
		if (newScore < 0)
			throw new ArgumentOutOfRangeException(nameof(newScore), "Score cannot be negative.");

		Score = newScore;
	}
}