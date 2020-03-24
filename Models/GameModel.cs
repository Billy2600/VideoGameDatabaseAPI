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
        [ForeignKey("PublisherId")]
        public int? PublisherId { get; set; }
        [ForeignKey("GenreId")]
        public int? GenreId { get; set; }

        [NotMapped]
        public string PublisherName { get; set; }
        [NotMapped]
        public string GenreName { get; set; }
    }
}
