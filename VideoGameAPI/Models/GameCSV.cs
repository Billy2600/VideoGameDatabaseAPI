using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoGameAPI.Models
{
    // Model for importing game from CSV
    public class GameCSV
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }
        public string Console { get; set; }
    }
}
