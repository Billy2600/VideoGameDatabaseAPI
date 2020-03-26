using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameAPI.Models;

namespace VideoGameAPI.Contexts
{
    [Table("Consoles")]
    public class ConsoleContext : DbContext
    {
        public virtual DbSet<ConsoleModel> Consoles { get; set; } // Virtual as to be overridden for unit testing

        public ConsoleContext() { }

        public ConsoleContext(DbContextOptions<ConsoleContext> options)
            : base(options)
        {

        }
    }
}
