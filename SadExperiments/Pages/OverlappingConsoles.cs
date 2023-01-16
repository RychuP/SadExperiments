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

        // keyboard handler for page changing
        Func<IScreenObject, SadConsole.Input.Keyboard, bool> keyboardHandler =
        (screenObject, keyboard) => this.ProcessKeyboard(keyboard);

        // First console
        _console1 = new(60, 14);
        _console1.Position = new Point(3, 2);
        _console1.DefaultBackground = Color.AnsiCyan;
        _console1.Clear();
        _console1.Print(1, 1, "Type on me!");
        _console1.Cursor.Position = new Point(1, 2);
        _console1.Cursor.IsEnabled = true;
        _console1.Cursor.IsVisible = true;
        _console1.Cursor.MouseClickReposition = true;
        _console1.FocusOnMouseClick = true;
        _console1.MoveToFrontOnMouseClick = true;
        _console1.WithKeyboard(keyboardHandler);

        // Add a child surface
        ScreenSurface surfaceObject = new ScreenSurface(5, 3);
        surfaceObject.Surface.FillWithRandomGarbage(surfaceObject.Font);
        surfaceObject.Position = _console1.Area.Center - (surfaceObject.Surface.Area.Size / 2);
        surfaceObject.UseMouse = false;

        _console1.Children.Add(surfaceObject);

        // Second console
        _console2 = new(58, 12);
        _console2.Position = new Point(19, 11);
        _console2.DefaultBackground = Color.AnsiRed;
        _console2.Clear();
        _console2.Print(1, 1, "Type on me!");
        _console2.Cursor.Position = new Point(1, 2);
        _console2.Cursor.IsEnabled = true;
        _console2.Cursor.IsVisible = true;
        _console2.FocusOnMouseClick = true;
        _console2.MoveToFrontOnMouseClick = true;
        _console2.WithKeyboard(keyboardHandler);

        // container.Children.Add(console2);
        // container.Children.MoveToBottom(console2);

        // order matters
        Children.Add(_console2);
        Children.Add(_console1);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Container)
        {
            (Children.Last() as Console)!.IsFocused = true;
        }
        base.OnParentChanged(oldParent, newParent);
    }
}