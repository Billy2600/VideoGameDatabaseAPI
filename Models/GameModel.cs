using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoGameAPI.Models
{
    public class GameModel
    {
        public int GameId { get; set; }
        public string GameName { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int PublisherId { get; set; }
        public int GenreId { get; set; }
    }
}
