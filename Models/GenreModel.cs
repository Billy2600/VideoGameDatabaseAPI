using System;
using System.ComponentModel.DataAnnotations;

namespace VideoGameAPI.Models
{
    public class GenreModel
    {
        [Key]
        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }
}
