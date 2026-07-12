using Microsoft.EntityFrameworkCore;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Entities;
using WorldRank.Domain.Enums;

namespace WorldRank.Infrastructures.Data
{
    public class DBWalletRepository : IWalletRepository
    {
        private readonly WorldRankDbContext _context;

        public DBWalletRepository(WorldRankDbContext context)
        {
            _context = context;
        }

        public void Add(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
        }

        public Wallet GetWallet(int playerId, Currency currency)
        {
            var wallet = _context.Wallets
                .FirstOrDefault(w => w.PlayerId == playerId && w.Currency == currency);
            if (wallet == null)
                throw new InvalidOperationException("Wallet not found.");
            return wallet;
        }

        public List<Wallet> GetAllWalletsByPlayerId(int playerId)
        {
            return _context.Wallets
                .Where(w => w.PlayerId == playerId)
                .ToList();
        }

        public void Deposit(int playerId, Currency currency, decimal amount)
        {
            var wallet = GetWallet(playerId, currency);
            wallet.Deposit(amount);
            _context.SaveChanges();
        }

        public void Withdraw(int playerId, Currency currency, decimal amount)
        {
            var wallet = GetWallet(playerId, currency);
            // Wallet.Withdraw will validate blocked state and balance
            wallet.Withdraw(amount);
            _context.SaveChanges();
        }

        public void UpdateBalance(int playerId, Currency currency, decimal newBalance)
        {
            var wallet = GetWallet(playerId, currency);
            wallet.SetBalance(newBalance);
            _context.SaveChanges();
        }

        public void Block(int playerId, Currency currency)
        {
            var wallet = GetWallet(playerId, currency);
            wallet.Block();
            _context.SaveChanges();
        }

        public void Unblock(int playerId, Currency currency)
        {
            var wallet = GetWallet(playerId, currency);
            wallet.Unblock();
            _context.SaveChanges();
        }
    }
}