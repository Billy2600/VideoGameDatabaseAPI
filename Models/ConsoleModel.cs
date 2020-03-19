using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
