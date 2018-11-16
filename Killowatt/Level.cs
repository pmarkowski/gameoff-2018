using GoRogue.MapViews;
using GoRogue.Pathing;
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
        AStar aStar;
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
            aStar = new GoRogue.Pathing.AStar(map, GoRogue.Distance.MANHATTAN);

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
            return new Player(playerPos.X, playerPos.Y, '@', new Energy(100, 20));
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

        internal void Step()
        {
            // Enemies that are adjacent to the player attempt to attack
            // Otherwise they try to move towards the player
            foreach (Enemy enemy in Enemies)
            {
                Path path = aStar.ShortestPath(enemy.X, enemy.Y, Player.X, Player.Y);
                if (path.Length > 1)
                {
                    GoRogue.Coord nextPoint = path.GetStep(0);
                    enemy.X = nextPoint.X;
                    enemy.Y = nextPoint.Y;
                    enemy.RenderEntity.Position = new Microsoft.Xna.Framework.Point(nextPoint.X, nextPoint.Y);
                }
            }
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
                
            if (attackRoll > 5) // TODO: make this a value based on the defender's AC
            {
                int damageRoll = GoRogue.DiceNotation.Dice.Roll("1d6");
                defender.Energy.ConsumeEnergy(damageRoll); // Damage is dealt to energy
                if (!defender.Energy.HasEnergy())
                {
                    logger.LogMessage($"Hit for {damageRoll} damage, defeating the enemy!");
                }
                else
                {
                    logger.LogMessage($"Hit for {damageRoll} damage!");
                }
            }
            else
            {
                logger.LogMessage($"Your attack missed!");
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
                logger.LogMessage("I feel completely recharged!");
            }
        }
    }
}
