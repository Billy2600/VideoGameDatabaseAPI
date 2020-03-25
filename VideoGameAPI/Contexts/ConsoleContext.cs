using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameAPI.Models;

namespace VideoGameAPI.Contexts
{
    [Table("Consoles")]
    public class ConsoleContext : DbContext
    {
        public DbSet<ConsoleModel> Consoles { get; set; }

        public ConsoleContext(DbContextOptions<ConsoleContext> options)
            : base(options)
        {

        }
    }
}
