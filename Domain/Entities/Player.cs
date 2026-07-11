namespace WorldRank.Domain.Entities;

public class Player : IPlayer
{
    // Public setters are required by the IPlayer contract and for EF materialization.
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Score { get; set; }

    // Parameterless ctor for EF / deserialization
    public Player()
    {
    }

    public Player(int id, string name, int score)
    {
        ID = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Score = score;
    }

    public Player(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        ID = id;
        Name = name;
        Score = 0;
    }

    public void AddScore(int points)
    {
        if (points < 0)
            throw new ArgumentOutOfRangeException(nameof(points), "Points cannot be negative.");

        Score += points;
    }

    public override string ToString() => $"[{ID}] {Name} - Score: {Score}";
}
