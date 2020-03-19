using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameAPI.Models
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
