using WorldRank;


public class Player : IPlayer
{
    private Dictionary<Currency, Wallet> wallets = [];

    public enum Currency
    {
        EUR,
        USD,
        GBP
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public Dictionary<Currency, Wallet> Wallets { get => wallets; set => wallets = value; }
    public Player(int id, string name)
    {
        Name = name;
        Id = id;
    }

    public Player()
    {
    }

    public Player(string name)
    {
        Name = name;
    }

    public override string ToString() => $"[{Id}] {Name} - Score: {Score}";

    public void UpdateScore(int newScore)
    {
        if (newScore < 0)
            throw new ArgumentOutOfRangeException(nameof(newScore), "Score cannot be negative.");

        Score = newScore;
    }
}