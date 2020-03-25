using System;
using System.ComponentModel.DataAnnotations;

namespace VideoGameAPI.Models
{
    public class ConsoleModel
    {
        [Key]
        public int ConsoleId { get; set; }
		public string ConsoleName { get; set; }
		public DateTime ReleaseDate { get; set; }
        public string Manufacturer { get; set; }
		public int UnitsSold { get; set; }
    }
}
