global using Console = SadConsole.Console;
global using SadRogue.Primitives;
global using SadConsole;

namespace TestProject;

class Program
{
    public const int Width = 80;
    public const int Height = 25;

    static void Main(string[] args)
    {
        // Setup the engine and create the main window.
        Game.Create(Width, Height);

        // Hook the start event so we can add consoles to the system.
        Game.Instance.OnStart = Init;

        // Start the game.
        Game.Instance.Run();
        Game.Instance.Dispose();
    }

    private static void Init()
    {
        Game.Instance.Screen = new Test();
        Game.Instance.DestroyDefaultStartingConsole();
    }
}