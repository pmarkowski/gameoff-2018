using SadConsole.Entities;

namespace Killowatt
{
    public abstract class MapObject
    {
        public Entity RenderEntity { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public MapObject(int x, int y, char glyph)
        {
            X = x;
            Y = y;

            RenderEntity = new Entity(1, 1);
            RenderEntity.Position = new Microsoft.Xna.Framework.Point(x, y);
            RenderEntity.Animation.CurrentFrame[0].Glyph = glyph;
        }
    }
}