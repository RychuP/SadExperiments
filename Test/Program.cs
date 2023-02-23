using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
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

internal class Test : ScreenSurface
{
    SadConsole.Components.Timer _timer;
    int _index = 0;

    public Test() : base(GameHost.Instance.ScreenCellsX, GameHost.Instance.ScreenCellsY)
    {
        _timer = new(TimeSpan.FromSeconds(0.1d));
        SadComponents.Add(_timer);
        _timer.TimerElapsed += Timer_OnTimerElapsed;
        IsFocused = true;
    }

    void Timer_OnTimerElapsed(object? sender, EventArgs e)
    {
        if (_index == Surface.Count)
        {
            _index = -1;
            Surface.Clear();
        }
        Surface[_index++].GlyphCharacter = '.';
        IsDirty = true;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Keys.Space))
        {
            if (_timer.IsPaused)
            {
                _timer.Repeat = true;
                _timer.Restart();
            }
            else
            {
                _timer.Repeat = false;
                _timer.IsPaused = true;
            }
        }
        return base.ProcessKeyboard(keyboard);
    }
}