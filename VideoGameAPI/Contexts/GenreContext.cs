using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameAPI.Models;

namespace VideoGameAPI.Contexts
{
    [Table("Genres")]
    public class GenreContext : DbContext
    {
        public virtual DbSet<GenreModel> Genres { get; set; } // Virtual as to be overridden for unit testing

        public GenreContext(DbContextOptions<GenreContext> options)
            : base(options)
        {

        }
    }
}
