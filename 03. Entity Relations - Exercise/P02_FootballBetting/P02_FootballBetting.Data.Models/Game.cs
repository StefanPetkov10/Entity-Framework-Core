using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    public class Game
    {
        public Game()
        {
            this.Bets = new HashSet<Bet>();
            this.PlayersStatistics = new HashSet<PlayerStatistic>();
        }

        //In real project it is good the PK to be string -> Guid
        [Key]
        public int GameId { get; set; }

        [ForeignKey(nameof(HomeTeam))]
        public int HomeTeamId { get; set; }
        public virtual Team HomeTeam { get; set; } = null!;

        [ForeignKey(nameof(AwayTeam))]
        public int AwayTeamId { get; set; }
        public virtual Team AwayTeam { get; set; } = null!;

        public byte HomeTeamGoals { get; set; }

        public byte AwayTeamGoals { get; }

        //DataTime is required by default and it is not nullable
        //DataTime? is nullable
        public DateTime DateTime { get; set; }

        public double HomeTeamBetRate { get; set; }

        public double AwayTeamBetRate { get; set; }

        public double DrawBetRate { get; set; }

        [MaxLength(ValidationConstants.UserUserNameMaxLength)]
        public string? Result { get; set; }

        public virtual ICollection<Bet> Bets { get; set; }
        public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; }
    }
}
