namespace WorldRank.Console
{
	public interface IPlayerRepository
	{
		void AddPlayer(Player player);

		void DeletePlayer(int playerId);

		Player FindPlayer(int playerId);

		IEnumerable<IGrouping<int, Player>> GroupPlayersByScore();
	}
}