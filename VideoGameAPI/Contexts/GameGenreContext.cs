using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameAPI.Models;

namespace VideoGameAPI.Contexts
{
    [Table("GamesGenres")]
    public class GameGenreContext : DbContext
    {
        public virtual DbSet<GameGenreModel> GamesGenres { get; set; } // Virtual as to be overridden for unit testing

        public GameGenreContext() { }

        public GameGenreContext(DbContextOptions<GameGenreContext> options)
            : base(options)
        {

        }
    }
}
