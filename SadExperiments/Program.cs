global using System;
global using System.Linq;
global using System.Collections.Generic;
global using SadConsole;
global using SadConsole.Input;
global using SadRogue.Primitives;
global using SadExperiments.MainScreen;
global using Console = SadConsole.Console;
global using MonoColor = Microsoft.Xna.Framework.Color;
global using Keyboard = SadConsole.Input.Keyboard;
using SadConsole.UI;

namespace SadExperiments;

public static class Program
{
    public const int Width = 80;
    public const int Height = 30;

    static void Main()
    {
        Settings.WindowTitle = "SadConsole Experiments";
        Settings.ResizeMode = Settings.WindowResizeOptions.Fit;
        RegistrarExtended.Register();
        Game.Create(Width, Height + Header.MinimizedViewHeight); 
        Game.Instance.OnStart = Init;
        Game.Instance.Run();
        Game.Instance.Dispose();
    }

    static void Init() =>
        Container.Root.Init();

    public static Color RandomColor => 
        Color.White.GetRandomColor(Game.Instance.Random);
}