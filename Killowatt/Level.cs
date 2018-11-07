using GoRogue.MapViews;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Killowatt
{
    public class Level
    {
        int width, height;
        ArrayMap<bool> map;

        public Player Player { get; private set; }

        public Level(int width, int height)
        {
            this.width = width;
            this.height = height;
            map = new ArrayMap<bool>(width, height);
            //GoRogue.MapGeneration.Generators.RectangleMapGenerator.Generate(map);

            GoRogue.MapGeneration.Generators.RandomRoomsGenerator.Generate(map, 10, 3, 20, 5);

            // Place player
            GoRogue.Coord playerPos = map.RandomPosition(true);
            Player = new Player()
            {
                X = playerPos.X,
                Y = playerPos.Y
            };
        }

        internal Cell[] GetCells()
        {
            Cell[] cells = new Cell[width * height];
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    Cell cell = new Cell();
                    if (map[i, j])
                    {
                        cell.Glyph = '.';
                    }
                    else
                    {
                        cell.Glyph = '#';
                    }
                    cells[i + (j * width)] = cell;
                }
            }
            return cells;
        }

        internal bool SquareIsPassable(int x, int y)
        {
            return map[x, y];
        }
    }
}
