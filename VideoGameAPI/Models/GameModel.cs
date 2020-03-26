using System;
using System.Collections.Generic;
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
        [ForeignKey("ConsoleId")]
        public int? ConsoleId { get; set; }

        [NotMapped]
        public string PublisherName { get; set; }
        [NotMapped]
        public string GenreName { get; set; }
        [NotMapped]
        public string ConsoleName { get; set; }
        [NotMapped]
        public List<string> Genres { get; set; }

        public static GameModel operator +(GameModel gameA, GameModel gameB)
        {
            var newGame = new GameModel
            {
                GameId = gameA.GameId == 0 ? gameB.GameId : gameA.GameId,
                GameName = gameA.GameName == null ? gameB.GameName : gameA.GameName,
                ReleaseDate = gameA.ReleaseDate == null ? gameB.ReleaseDate : gameA.ReleaseDate,
                PublisherId = gameA.PublisherId == null ? gameB.PublisherId : gameA.PublisherId,
                PublisherName = gameA.PublisherName == null ? gameB.PublisherName : gameA.PublisherName,
                GenreName = gameA.GenreName == null ? gameB.GenreName : gameA.GenreName,
                ConsoleId = gameA.ConsoleId == null ? gameB.ConsoleId : gameA.ConsoleId,
                ConsoleName = gameA.ConsoleName == null ? gameB.ConsoleName : gameA.ConsoleName,
                Genres = gameA.Genres == null ? gameB.Genres : gameA.Genres
            };

            return newGame;
        }
    }
}
