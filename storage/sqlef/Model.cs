using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
//using System.Data.Entity.Migrations;

namespace LinuxAcademy.AZ200.EntityFrameworkSamples
{

    // https://docs.microsoft.com/en-us/ef/core/modeling/relationships

    public class Player
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        // old way
        //  public virtual ICollection<Game> Games { get; set; }
        public ICollection<PlayerGame> PlayerGames { get; set; } = new List<PlayerGame>();
    }

    public class Game
    {
        public int GameId { get; set; }
        public string Title { get; set; }
        public string Platform { get; set; }
        //public virtual ICollection<Player> Players { get; set; }
        public ICollection<PlayerGame> PlayerGames { get; set; } = new List<PlayerGame>();
    }

    public class PlayerGame
    {
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
    }

    public class GamingSystemDbContext : DbContext
    {
        public GamingSystemDbContext() { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerGame> PlayerGames { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:laaz200efsql.database.windows.net,1433;Initial Catalog=entities;Persist Security Info=False;User ID=ServerAdmin;Password=Change!Me!Please;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // old way
            // modelBuilder.Entity<Player>().HasMany<Game>(p => p.Games).WithMany(g => g.Players);

            modelBuilder.Entity<PlayerGame>().HasKey(pg => new { pg.PlayerId, pg.GameId });

            modelBuilder.Entity<PlayerGame>()
                .HasOne(pg => pg.Player)
                .WithMany(p => p.PlayerGames)
                .HasForeignKey(pg => pg.PlayerId);
            modelBuilder.Entity<PlayerGame>()
                .HasOne(pg => pg.Game)
                .WithMany(g => g.PlayerGames)
                .HasForeignKey(pg => pg.GameId);
        }
    }
}