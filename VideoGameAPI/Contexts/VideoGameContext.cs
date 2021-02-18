using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using VideoGameAPI.Models;

namespace VideoGameAPI.Contexts
{
    [Table("Consoles")]
    public class VideoGameContext : DbContext
    {
        public virtual DbSet<ConsoleModel> Consoles { get; set; } // Virtual as to be overridden for unit testing
        public virtual DbSet<GameModel> Games { get; set; }
        public virtual DbSet<GameGenreModel> GamesGenres { get; set; }
        public virtual DbSet<GenreModel> Genres { get; set; }
        public virtual DbSet<PublisherModel> Publishers { get; set; }

        public VideoGameContext() { }

        public VideoGameContext(DbContextOptions<VideoGameContext> options)
            : base(options)
        {

        }

        // So it can be mocked out in unit tests
        virtual public int GetConsoleIdByName(string consoleName)
        {
            return Consoles.Where(x => x.ConsoleName == consoleName).FirstOrDefault().ConsoleId;
        }

        // Additional level of abstraction for unit testing
        public virtual void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public async Task<int> GetPublisherIdByName(string publisherName)
        {
            var publisher = await Publishers.Where(x => x.PublisherName == publisherName).FirstOrDefaultAsync();

            if (publisher == null)
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
