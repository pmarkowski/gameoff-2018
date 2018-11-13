using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Killowatt
{
    public class ChargeStation : MapObject
    {
        public ChargeStation(int x, int y, char glyph)
            : base(x, y, glyph)
        {
        }
    }
}
