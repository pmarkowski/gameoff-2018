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
        private const int MaxRooms = 20;
        private const int MinRoomSize = 3;
        private const int MaxRoomSize = 40;

        int width, height;
        ArrayMap<bool> map;

        public List<ChargeStation> ChargeStations { get; set; }
        public Player Player { get; private set; }

        public Level(int width, int height)
        {
            this.width = width;
            this.height = height;
            map = new ArrayMap<bool>(width, height);
            //GoRogue.MapGeneration.Generators.RectangleMapGenerator.Generate(map);

            GoRogue.MapGeneration.Generators.RandomRoomsGenerator.Generate(map, MaxRooms, MinRoomSize, MaxRoomSize, 5);

            // Place player
            GoRogue.Coord playerPos = map.RandomPosition(true);
            Player = new Player()
            {
                X = playerPos.X,
                Y = playerPos.Y,
                Energy = new Energy(20, 5)
            };

            // Generate charge stations
            ChargeStations = new List<ChargeStation>();
            // How Many? as many as max rooms for now
            for (int i = 0; i < MaxRooms; i++)
            {
                GoRogue.Coord chargePos = map.RandomPosition((coord, value) =>
                    value &&
                    coord != playerPos &&
                    !ChargeStations.Any(station =>
                        coord == GoRogue.Coord.Get(station.X, station.Y)));
                ChargeStations.Add(new ChargeStation()
                {
                    X = chargePos.X,
                    Y = chargePos.Y
                });
            }
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

        internal void PlayerMoved(int x, int y)
        {
            Player.X = x;
            Player.Y = y;
            Player.Energy.ConsumeEnergy(1);
        }

        internal void PlayerInteract()
        {
            if (ChargeStations.Any(station => station.X == Player.X && station.Y == Player.Y))
            {
                Player.Energy.CurrentSoc = Player.Energy.MaxSoc;
            }
        }
    }
}
