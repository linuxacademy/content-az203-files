using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LinuxAcademy.AZ200.EntityFrameworkSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            // initdb();
            // insert();
            get();
        }

        public static void initdb()
        {
            using (var context = new GamingSystemDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }

        public static void insert()
        {
            using (var context = new GamingSystemDbContext())
            {
                var game = new Game() { Title = "Halo", Platform = "XBox" };
                var player = new Player { PlayerName = "Mike" };
                var playerGame = new PlayerGame { 
                    PlayerId = player.PlayerId,
                    Player = player,
                    GameId = game.GameId,
                    Game = game
                };
                player.PlayerGames.Add(playerGame);
                game.PlayerGames.Add(playerGame);
                context.Players.Add(player);
                context.SaveChanges();
            }
        }

        public static void get()
        {
            using (var db = new GamingSystemDbContext())
            {
                var player = db.Players
                    .Include(p => p.PlayerGames)
                    .ThenInclude(pg => pg.Game)
                    .FirstOrDefaultAsync(p => p.PlayerName == "Mike").Result;
                player = null;
            }
        }

        public static Game GetGame(int gameId)
        {
            using (var db = new GamingSystemDbContext())
            {
                return db.Games.FirstOrDefaultAsync(g => g.GameId == gameId).Result;
            }
        }

        public static Player GetPlayer(int playerId)
        {
            using (var db = new GamingSystemDbContext())
            {
                return db.Players
                .Include(p => p.PlayerGames)
                .ThenInclude(pg => pg.Game)
                .FirstOrDefaultAsync(p => p.PlayerId == playerId).Result;
            }
        }
    }
}
