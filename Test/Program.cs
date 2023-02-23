using SadConsole;
using SadConsole.Components;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace MyProject;

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
        //var startingConsole = (Console)GameHost.Instance.Screen;
        GameHost.Instance.Screen = new Test();
    }
}

internal class Test : ScreenObject
{
    public Test()
    {
        var timer = new SadConsole.Components.Timer(TimeSpan.FromSeconds(0.1d));
        SadComponents.Add(timer);
        timer.TimerElapsed += Timer_OnTimerElapsed;
    }

    void Timer_OnTimerElapsed(object? sender, EventArgs e)
    {
        if (sender is SadConsole.Components.Timer t)
            t.IsPaused = true;
    }
}