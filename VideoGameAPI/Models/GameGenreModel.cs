using System;
using System.ComponentModel.DataAnnotations;

namespace VideoGameAPI.Models
{
    public class GameGenreModel
    {
        [Key]
        public int GameGenreId { get; set; }
        public int GameId { get; set; }
        public int GenreId { get; set; }
    }
}
