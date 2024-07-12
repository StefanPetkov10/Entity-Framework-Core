using Microsoft.EntityFrameworkCore;

using P02_FootballBetting.Data.Common;
using P02_FootballBetting.Data.Models;

namespace P02_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        //Use it when developing the application
        //When we test the application locally on your PC
        public FootballBettingContext()
        {

        }

        //Use by Judge
        //Loading of the DbContext WITH  DI -> In real application it is useful
        public FootballBettingContext(DbContextOptions options)
            : base(options)
        {

        }

        //DbSets
        public DbSet<Team> Teams { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PlayerStatistic> PlayersStatistics { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<User> Users { get; set; }


        // Connection configuration
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //Set the efault connection string
                //Someone used empty constructor of the DbContext
                optionsBuilder.UseSqlServer(DbConfig.ConnectionString);
            }
        }

        //Fluent API and Entities configuration8
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(e => new { e.GameId, e.PlayerId });
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasOne(e => e.PrimaryKitColor)
                    .WithMany(c => c.PrimaryKitTeams)
                    .HasForeignKey(e => e.PrimaryKitColorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.SecondaryKitColor)
                    .WithMany(c => c.SecondaryKitTeams)
                    .HasForeignKey(e => e.SecondaryKitColorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasOne(e => e.HomeTeam)
                    .WithMany(t => t.HomeGames)
                    .HasForeignKey(e => e.HomeTeamId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.AwayTeam)
                    .WithMany(t => t.AwayGames)
                    .HasForeignKey(e => e.AwayTeamId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}