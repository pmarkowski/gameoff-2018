using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Killowatt
{
    public class Player : Actor
    {
        public Player(int x, int y, char glyph, Energy energy)
            : base(x, y, glyph, energy)
        {
        }
    }
}
