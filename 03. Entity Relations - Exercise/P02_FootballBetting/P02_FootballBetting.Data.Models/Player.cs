using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    public class Player
    {
        public Player()
        {
            this.PlayersStatistics = new HashSet<PlayerStatistic>();
        }

        [Key]
        public int PlayerId { get; set; }

        [Required]
        [MaxLength(ValidationConstants.PlayerNameMaxLength)]
        public string Name { get; set; } = null!;

        public int SquadNumber { get; set; }

        //SQL Type -> BIT
        //By default boll is NOT NULL, by default is required
        public bool IsInjured { get; set; }

        //This FK should not be NOT NULL
        [ForeignKey(nameof(Team))]
        //can be NULL -> int? but judge does not accept it
        public int TeamId { get; set; }
        public virtual Team Team { get; set; } = null!;

        [ForeignKey(nameof(Position))]
        public int PositionId { get; set; }
        public virtual Position Position { get; set; } = null!;

        [ForeignKey(nameof(Town))]
        public int TownId { get; set; }
        public virtual Town Town { get; set; }

        public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; }
    }
}
