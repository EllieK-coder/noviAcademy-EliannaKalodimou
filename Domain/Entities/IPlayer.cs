

namespace WorldRank.Domain.Entities
{
    public interface IPlayer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
    }
}
