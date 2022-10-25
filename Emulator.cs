using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project300
{
    internal class Emulator
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Stability StabilityRating { get; set; }
        public string Image { get; set; }
        public string Console { get; set; }

        public enum Stability
        {
            Stable,
            Unstable,
            Experimental
        }
    }
}
