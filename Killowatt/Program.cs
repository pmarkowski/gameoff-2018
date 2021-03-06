using System;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Entities;
using GoRogue.MapViews;
using System.Linq;

namespace Killowatt
{
    class Program
    {
        public const int DisplayWidth = 80;
        public const int DisplayHeight = 30;
        public const int BottomConsoleWidth = 16;
        public const int BottomConsoleHeight = 2;

        private static Level map;
        private static Console levelConsole, energyConsole;

        static void Main(string[] args)
        {
            // Setup the engine and creat the main window.
            SadConsole.Game.Create("IBM.font", DisplayWidth, DisplayHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;
                        
            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //
            
            SadConsole.Game.Instance.Dispose();
        }
        
        private static void Update(GameTime time)
        {
            // Called each logic update.
            // This gets called repeatedly in real time
            // We only need to do any logic if game state is altered in some way
            if (!map.Player.Energy.HasEnergy())
            {
                Console gameOverConsole = new Console(DisplayWidth, DisplayHeight);
                gameOverConsole.FillWithRandomGarbage();
                SadConsole.Global.CurrentScreen.Children.Add(gameOverConsole);
            }

            // As an example, we'll use the F5 key to make the game full screen
            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                SadConsole.Settings.ToggleFullScreen();
            }

            bool progressState = false;
            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                Point newPoint = map.Player.RenderEntity.Position + new Point(0, -1);
                TryMovePlayer(map.Player.RenderEntity, newPoint);
                progressState = true;
            }
            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                Point newPoint = map.Player.RenderEntity.Position + new Point(0, 1);
                TryMovePlayer(map.Player.RenderEntity, newPoint);
                progressState = true;
            }
            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                Point newPoint = map.Player.RenderEntity.Position + new Point(-1, 0);
                TryMovePlayer(map.Player.RenderEntity, newPoint);
                progressState = true;
            }
            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                Point newPoint = map.Player.RenderEntity.Position + new Point(1, 0);
                TryMovePlayer(map.Player.RenderEntity, newPoint);
                progressState = true;
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
            {
                // Try to use whatever tile the player is on
                map.PlayerInteract();
                progressState = true;
            }

            // Sync entities
            map.UpdateEntities();
            levelConsole.CenterViewPortOnPoint(map.Player.RenderEntity.Position);
            RenderPlayerEnergy();

            if (progressState)
            {
                map.Step();
            }
        }

        private static void Init()
        {
            Console messageConsole = new Console(
                DisplayWidth - BottomConsoleWidth,
                BottomConsoleHeight);
            messageConsole.Position = new Point(BottomConsoleWidth, DisplayHeight - BottomConsoleHeight);

            GameMessageLogger logger = new GameMessageLogger(messageConsole);
            EntityManager entityManager = new EntityManager();
            Console parentConsole = new Console(DisplayWidth, DisplayHeight);

            // Generate a map
            int mapWidth = DisplayWidth * 2, mapHeight = DisplayHeight * 2;
            map = new Level(mapWidth, mapHeight, logger, entityManager);

            levelConsole = new Console(
                mapWidth,
                mapHeight,
                Global.FontDefault,
                new Rectangle(0, 1, DisplayWidth, DisplayHeight - BottomConsoleHeight),
                map.GetCells());

            parentConsole.Children.Add(levelConsole);

            energyConsole = new Console(
                BottomConsoleWidth,
                BottomConsoleHeight);
            energyConsole.Position = new Point(0, DisplayHeight - BottomConsoleHeight);
            parentConsole.Children.Add(energyConsole);

            RenderPlayerEnergy();

            parentConsole.Children.Add(messageConsole);

            // Set the player
            levelConsole.Children.Add(entityManager);
            levelConsole.CenterViewPortOnPoint(map.Player.RenderEntity.Position);

            // Set our new console as the thing to render and process
            SadConsole.Global.CurrentScreen = parentConsole;

            logger.LogMessage("Tread lightly.");
        }

        private static void TryMovePlayer(Entity player, Point nextPoint)
        {
            if (map.SquareHasEnemy(nextPoint.X, nextPoint.Y))
            {
                Enemy enemyAtNextPosition = map.Enemies.Where(enemy => enemy.X == nextPoint.X && enemy.Y == nextPoint.Y).Single();
                map.Attack(map.Player, enemyAtNextPosition);
            }
            else if (map.SquareIsPassable(nextPoint.X, nextPoint.Y))
            {
                player.Position = nextPoint;
                map.PlayerMoved(nextPoint.X, nextPoint.Y);
            }
        }

        private static void RenderPlayerEnergy()
        {
            energyConsole.Clear();
            energyConsole.Print(0, 0, $"Energy: {map.Player.Energy.CurrentSoc,3}/{map.Player.Energy.MaxSoc,-3}", Color.LightBlue);
            energyConsole.Print(0, 1, $"{"Fuel:",7} {map.Player.Energy.CurrentFuel,3}/{map.Player.Energy.MaxFuel,-3}", Color.LightGoldenrodYellow);
        }
    }
}
