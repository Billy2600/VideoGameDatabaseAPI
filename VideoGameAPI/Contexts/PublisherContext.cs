using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameAPI.Models;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<int> GetIdByName(string publisherName)
        {
            var publisher = await Publishers.Where(x => x.PublisherName == publisherName).FirstOrDefaultAsync();

            if(publisher == null)
            {
                Publishers.Add(new PublisherModel { PublisherName = publisherName });
                var newPublisher = await Publishers.Where(x => x.PublisherName == publisherName).FirstOrDefaultAsync();
                return newPublisher.PublisherId;
            }
            else
            {
                return publisher.PublisherId;
            }
        }
    }
}