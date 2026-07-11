using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Entities;

namespace WorldRank.Infrastructures.Data;

public class DBPlayerRepository : IPlayerRepository
{
    private readonly WorldRankDbContext _context;

    public DBPlayerRepository(WorldRankDbContext context)
    {
        _context = context;
    }

    public async Task<Player?> GetByIdAsync(int id)
    {
        return await _context.Players.FindAsync(id);
    }

    public async Task AddAsync(Player player)
    {
        await _context.Players.AddAsync(player);
        await _context.SaveChangesAsync();
    }

    public void AddPlayer(Player player)
    {
        _context.Players.Add(player);
        _context.SaveChanges();
    }

    public IEnumerable<Player> GetAllPlayers()
    {
        return _context.Players.AsNoTracking().ToList();
    }

    public void DeletePlayer(int playerId)
    {
        var player = _context.Players.Find(playerId);
        if (player == null) return;
        _context.Players.Remove(player);
        _context.SaveChanges();
    }

    public Player? FindPlayer(int playerId)
    {
        return _context.Players.Find(playerId);
    }

    public IEnumerable<IGrouping<int, Player>> GroupPlayersByScore()
    {
        return _context.Players.AsNoTracking().GroupBy(p => p.Score).ToList();
    }
}
