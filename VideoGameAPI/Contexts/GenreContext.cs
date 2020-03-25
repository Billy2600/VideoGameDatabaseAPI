using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameAPI.Models;

namespace VideoGameAPI.Contexts
{
    [Table("Genres")]
    public class GenreContext : DbContext
    {
        public DbSet<GenreModel> Genres { get; set; }

        public GenreContext(DbContextOptions<GenreContext> options)
            : base(options)
        {

        }
    }
}
