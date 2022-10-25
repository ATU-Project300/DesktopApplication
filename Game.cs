using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Project300.Emulator;

namespace Project300
{
    internal class Game
    {
        public string Title { get; set; }
        public int YearReleased { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string[] Consoles { get; set; }
        public Emulator Emulator { get; set; }
    }
}
