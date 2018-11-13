using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Killowatt
{
    public class Actor : MapObject
    {
        public Energy Energy { get; set; }

        public Actor(int x, int y, char glyph, Energy energy)
            : base(x, y, glyph)
        {
            Energy = energy;
        }
    }
}
