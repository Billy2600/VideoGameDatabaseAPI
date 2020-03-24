using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameAPI.Models;

namespace VideoGameAPI.Contexts
{
    [Table("Publishers")]
    public class PublisherContext : DbContext
    {
        public DbSet<PublisherModel> Publishers { get; set; }

        public PublisherContext(DbContextOptions<PublisherContext> options)
            : base(options)
        {

        }
    }
}