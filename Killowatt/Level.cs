using GoRogue.MapViews;
using SadConsole;
using SadConsole.Entities;
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
        GameMessageLogger logger;
        EntityManager entityManager;

        public List<ChargeStation> ChargeStations { get; set; }
        public List<Enemy> Enemies { get; set; }
        public Player Player { get; private set; }

        public Level(int width, int height, GameMessageLogger logger, EntityManager entityManager)
        {
            this.entityManager = entityManager;
            this.logger = logger;
            this.width = width;
            this.height = height;
            map = new ArrayMap<bool>(width, height);
            //GoRogue.MapGeneration.Generators.RectangleMapGenerator.Generate(map);

            GoRogue.MapGeneration.Generators.RandomRoomsGenerator.Generate(map, MaxRooms, MinRoomSize, MaxRoomSize, 5);

            // Place player
            Player = GeneratePlayer();
            entityManager.Entities.Add(Player.RenderEntity);

            // Generate charge stations
            ChargeStations = new List<ChargeStation>();
            // How Many? as many as max rooms for now
            for (int i = 0; i < MaxRooms; i++)
            {
                ChargeStation chargeStation = GenerateChargeStation();
                ChargeStations.Add(chargeStation);
                entityManager.Entities.Add(chargeStation.RenderEntity);

            }

            // Generate enemies
            Enemies = new List<Enemy>();
            for (int i = 0; i < MaxRooms / 3; i++)
            {
                Enemy enemy = GenerateEnemy();
                Enemies.Add(enemy);
                entityManager.Entities.Add(enemy.RenderEntity);
            }
        }

        private Player GeneratePlayer()
        {
            GoRogue.Coord playerPos = map.RandomPosition(true);
            return new Player(playerPos.X, playerPos.Y, '@', new Energy(20, 5));
        }

        private ChargeStation GenerateChargeStation()
        {
            GoRogue.Coord chargePos = map.RandomPosition((coord, value) =>
                    value &&
                    coord != GoRogue.Coord.Get(Player.X, Player.Y) &&
                    !ChargeStations.Any(station =>
                        coord == GoRogue.Coord.Get(station.X, station.Y)));

            return new ChargeStation(chargePos.X, chargePos.Y, '&');
        }

        private Enemy GenerateEnemy()
        {
            GoRogue.Coord chargePos = map.RandomPosition((coord, value) =>
                    value &&
                    coord != GoRogue.Coord.Get(Player.X, Player.Y) &&
                    !ChargeStations.Any(station =>
                        coord == GoRogue.Coord.Get(station.X, station.Y)));

            return new Enemy(chargePos.X, chargePos.Y, 'D', new Energy(0, 5)); // TODO: Define enemy fuel levels better
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

        internal void UpdateEntities()
        {
            var enemiesToRemove = Enemies.Where(enemy => !enemy.Energy.HasEnergy()).ToList();
            foreach (Enemy enemy in enemiesToRemove)
            {
                if (!enemy.Energy.HasEnergy())
                {
                    entityManager.Entities.Remove(enemy.RenderEntity);
                    Enemies.Remove(enemy);
                }
            }
        }

        internal bool SquareIsPassable(int x, int y)
        {
            return map[x, y] && !Enemies.Any(enemy => enemy.X == x && enemy.Y == y);
        }

        internal bool SquareHasEnemy(int x, int y)
        {
            return Enemies.Any(enemy => enemy.X == x && enemy.Y == y);
        }

        internal void Attack(Actor attacker, Actor defender)
        {
            // Roll to hit
            int attackRoll = GoRogue.DiceNotation.Dice.Roll("1d20");
            logger.LogMessage($"Rolled {attackRoll} to hit");
                
            if (attackRoll > 5) // TODO: make this a value based on the defender's AC
            {
                int damageRoll = GoRogue.DiceNotation.Dice.Roll("1d6");
                defender.Energy.ConsumeEnergy(damageRoll); // Damage is dealt to energy
                logger.LogMessage($"Hit for {damageRoll} damage!");
                if (!defender.Energy.HasEnergy())
                {
                    logger.LogMessage($"Defender is out of energy!");
                }
            }
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
