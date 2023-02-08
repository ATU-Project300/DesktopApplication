using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odyssey
{
    public class Game
    {
        public string Title;
        public int Year;
        public string Description;
        public string Image;
        public string Consoles;
        public string Emulator;
        public string LocalPath; // Only assigned and saved locally, will never be part of API
    }
}
