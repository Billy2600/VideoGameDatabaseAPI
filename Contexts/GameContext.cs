using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameAPI.Models;

namespace VideoGameAPI.Contexts
{
    [Table("Games")]
    public class GameContext : DbContext
    {
        public DbSet<GameModel> Games { get; set; }

        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        {

        }
    }
}