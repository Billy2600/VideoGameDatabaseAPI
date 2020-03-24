using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameAPI.Models
{
    public class GameModel
    {
        [Key]
        public int GameId { get; set; }
        public string GameName { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int? PublisherId { get; set; }
        public int? GenreId { get; set; }

        [NotMapped]
        public string PublisherName { get; set; }
        [NotMapped]
        public string GenreName { get; set; }
    }
}
