global using System;
global using System.Linq;
global using System.Collections.Generic;
global using SadConsole;
global using SadConsole.Input;
global using SadRogue.Primitives;
global using SadExperiments.MainScreen;
global using Console = SadConsole.Console;
global using MonoColor = Microsoft.Xna.Framework.Color;

namespace SadExperiments;

public static class Program
{
    public const int Width = 80;
    public const int Height = 30;

    static void Main()
    {
        Settings.WindowTitle = "SadConsole Experiments";
        Settings.ResizeMode = Settings.WindowResizeOptions.Fit;
        Game.Create(Width, Height + Header.Height);
        Game.Instance.OnStart = Init;
        Game.Instance.Run();
        Game.Instance.Dispose();
    }

    static void Init()
    {
        _ = new Container();
    }

    public static Color RandomColor => Color.White.GetRandomColor(Game.Instance.Random);
}