using Microsoft.EntityFrameworkCore;
using WorldRank.Domain.Entities;

namespace WorldRank.Infrastructures.Data
{
    public class WorldRankDbContext : DbContext
    {
        public WorldRankDbContext(DbContextOptions<WorldRankDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.ID);
                entity.Property(p => p.ID).ValueGeneratedNever();
                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.Score);
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                // Wallet has no single Id field. A player has one wallet per currency,
                // so the natural (composite) key is PlayerId + Currency — which also
                // matches the uniqueness rule your repositories enforce.
                entity.HasKey(w => new { w.PlayerId, w.Currency });

                entity.Property(w => w.Currency).HasConversion<string>();
                entity.Property(w => w.Balance).HasColumnType("decimal(18,2)");
                entity.Property(w => w.IsBlocked);

                entity.HasOne<Player>()
                      .WithMany()
                      .HasForeignKey(w => w.PlayerId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
    