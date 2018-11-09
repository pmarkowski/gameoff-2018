using System;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Entities;
using GoRogue.MapViews;

namespace Killowatt
{
    class Program
    {
        public const int DisplayWidth = 80;
        public const int DisplayHeight = 30;

        private static Entity player;
        private static Level map;
        private static Console startingConsole;

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

            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                Point newPoint = player.Position + new Point(0, -1);
                TryMovePlayer(player, newPoint);
            }
            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                Point newPoint = player.Position + new Point(0, 1);
                TryMovePlayer(player, newPoint);
            }
            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                Point newPoint = player.Position + new Point(-1, 0);
                TryMovePlayer(player, newPoint);
            }
            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                Point newPoint = player.Position + new Point(1, 0);
                TryMovePlayer(player, newPoint);
            }

            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
            {
                // Try to use whatever tile the player is on
                map.PlayerInteract();
            }

            // Sync entities
        }

        private static void Init()
        {
            int mapWidth = DisplayWidth * 2, mapHeight = DisplayHeight * 2;
            // Generate a map
            map = new Level(mapWidth, mapHeight);

            CreatePlayer(map.Player);

            EntityManager entityManager = new EntityManager();
            entityManager.Entities.Add(player);

            foreach (ChargeStation chargeStation in map.ChargeStations)
            {
                Entity stationEntity = new Entity(1, 1);
                stationEntity.Position = new Point(chargeStation.X, chargeStation.Y);
                stationEntity.Animation.CurrentFrame[0].Glyph = '&';
                entityManager.Entities.Add(stationEntity);
            }

            startingConsole = new Console(
                mapWidth,
                mapHeight,
                Global.FontDefault,
                new Rectangle(0, 0, DisplayWidth, DisplayHeight),
                map.GetCells());

            // Set the player
            startingConsole.Children.Add(entityManager);
            startingConsole.CenterViewPortOnPoint(player.Position);

            // Set our new console as the thing to render and process
            SadConsole.Global.CurrentScreen = startingConsole;
        }

        private static void CreatePlayer(Player mapPlayer)
        {
            player = new Entity(1, 1);
            player.Position = new Point(mapPlayer.X, mapPlayer.Y);
            player.Animation.CurrentFrame[0].Glyph = '@';
        }

        private static void TryMovePlayer(Entity player, Point nextPoint)
        {
            if (map.SquareIsPassable(nextPoint.X, nextPoint.Y))
            {
                player.Position = nextPoint;
                map.PlayerMoved(nextPoint.X, nextPoint.Y);
                startingConsole.CenterViewPortOnPoint(player.Position);
            }
        }
    }
}
