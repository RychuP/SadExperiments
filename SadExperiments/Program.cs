global using System;
global using System.Linq;
global using System.Collections.Generic;
global using SadConsole;
global using SadConsole.Input;
global using SadRogue.Primitives;
global using SadExperiments.MainScreen;
global using Console = SadConsole.Console;
global using MonoColor = Microsoft.Xna.Framework.Color;

namespace SadExperiments
{
    // Tests of various features of SadConsole. 
    public static class Program
    {
        public const int Width = 80;
        public const int Height = 30;

        static void Main()
        {
            Settings.WindowTitle = "SadConsole Experiments";
            Settings.ResizeMode = Settings.WindowResizeOptions.Fit;

            // Setup the engine and create the main window.
            Game.Create(Width, Height + Header.Height);

            // Hook the start event so we can add consoles to the system.
            Game.Instance.OnStart = Init;

            // Start the game.
            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        static void Init()
        {
            _ = new Container();
        }

        public static Color RandomColor => Color.White.GetRandomColor(Game.Instance.Random);
    }
}
