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
        public const int Width = 80;
        public const int Height = 30;

        private static Entity player;

        static void Main(string[] args)
        {
            // Setup the engine and creat the main window.
            SadConsole.Game.Create("IBM.font", Width, Height);

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

            // As an example, we'll use the F5 key to make the game full screen
            if (SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                SadConsole.Settings.ToggleFullScreen();
            }

            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                player.Position += new Point(0, -1);
            }
            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                player.Position += new Point(0, 1);
            }
            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                player.Position += new Point(-1, 0);
            }
            else if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                player.Position += new Point(1, 0);
            }
        }

        private static void Init()
        {
            // Generate a map
            Map map = new Map(Width, Height);

            Console startingConsole = new Console(
                Width,
                Height,
                Global.FontDefault,
                new Rectangle(0, 0, Width, Height),
                map.GetCells());

            // Set the player
            CreatePlayer();
            startingConsole.Children.Add(player);

            

            // Set our new console as the thing to render and process
            SadConsole.Global.CurrentScreen = startingConsole;
        }

        private static void CreatePlayer()
        {
            player = new Entity(1, 1);
            player.Animation.CurrentFrame[0].Glyph = '@';
        }
    }
}
