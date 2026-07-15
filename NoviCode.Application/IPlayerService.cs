namespace NoviCode;

public interface IPlayerService
{
	Task<Player> CreateAsync(string name, int score, CancellationToken cancellationToken = default);
	Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken cancellationToken = default);
}
