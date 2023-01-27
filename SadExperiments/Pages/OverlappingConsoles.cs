using SadConsole.Components;
using SadConsole.Quick;

namespace SadExperiments.Pages;

internal class OverlappingConsoles : Page
{
    readonly Console _console1, _console2;

    public OverlappingConsoles()
    {
        Title = "Overlapping Consoles";
        Summary = "Following Thraka's tutorials: part 2, parenting.";

        // print prompts at the bottom of the page
        int y = Height - 5;
        Surface.Print(y, "Click to move consoles to the top and set focus.");
        ColoredString t = ColoredString.Parser.Parse("[c:r f:LightGreen]MouseClickReposition[c:undo] is true on the blue console.");
        int x = Width / 2 - t.Length / 2;
        Surface.Print(x, y + 2, t);

        // First console
        _console1 = new(60, 14)
        {
            Position = new Point(3, 2),
            DefaultBackground = Color.AnsiCyan
        };
        ConfigureConsole(_console1);
        ConfigureCursor(_console1.Cursor);
        _console1.Cursor.MouseClickReposition = true;

        // Add a child surface
        ScreenSurface surfaceObject = new ScreenSurface(5, 3);
        surfaceObject.Surface.FillWithRandomGarbage(surfaceObject.Font);
        surfaceObject.Position = _console1.Area.Center - (surfaceObject.Surface.Area.Size / 2);
        surfaceObject.UseMouse = false;

        _console1.Children.Add(surfaceObject);

        // Second console
        _console2 = new(58, 12)
        {
            Position = new Point(19, 11),
            DefaultBackground = Color.AnsiRed
        };
        ConfigureConsole(_console2);
        ConfigureCursor(_console2.Cursor);

        // order matters
        Children.Add(_console2);
        Children.Add(_console1);
    }

    // keyboard handler for page changing
    bool KeyboardHandler(IScreenObject so, Keyboard keyboard) => this.ProcessKeyboard(keyboard);

    void ConfigureConsole(Console console)
    {
        console.Clear();
        console.Print(1, 1, "Type on me!");
        console.WithKeyboard(KeyboardHandler);
        console.FocusOnMouseClick = true;
        console.MoveToFrontOnMouseClick = true;
    }

    static void ConfigureCursor(Cursor cursor)
    {
        cursor.Position = new Point(1, 2);
        cursor.IsEnabled = true;
        cursor.IsVisible = true;
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Container)
        {
            int last = Children.Count - 1;
            if (last > 0 && Children[last] is Console c)
                c.IsFocused = true;
        }
        base.OnParentChanged(oldParent, newParent);
    }
}