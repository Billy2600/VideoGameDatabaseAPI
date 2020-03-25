using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameAPI.Models;

namespace VideoGameAPI.Contexts
{
    [Table("Games")]
    public class GameContext : DbContext
    {
        public virtual DbSet<GameModel> Games { get; set; } // Virtual as to be overridden for unit testing

        public GameContext() { }

        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        {

        }
    }
}