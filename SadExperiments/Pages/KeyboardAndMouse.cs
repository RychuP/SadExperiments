using SadExperiments.Components;

namespace SadExperiments.Pages;

internal class KeyboardAndMouse : Page
{
    readonly Console _console;

    public KeyboardAndMouse()
    {
        Title = "Keyboard And Mouse";
        Summary = "Input handling via components or events.";

        (int Y, string Text)[] prompts = {
            (2, "Press 'Space' to change color."),
            (4, "Hover mouse over consoles"),
            (5, "to display position."),
            (7, "Click to reposition cursor."),
            (9, "Typing is turned off.")
        };
        Array.ForEach(prompts, p => Surface.Print(45, p.Y, p.Text));
        //Surface.Print(45, 2, "Press 'Space' to change color.");
        //Surface.Print(45, 4, "Hover mouse over consoles");
        //Surface.Print(45, 5, "to display position.");
        //Surface.Print(45, 7, "Click to reposition cursor.");
        //Surface.Print(45, 9, "Typing is turned off.");

        _console = new(30, 15);
        _console.Position = new Point(10, 1);
        _console.DefaultBackground = Color.AnsiGreen;
        _console.Clear();
        _console.IsFocused = true;
        _console.Cursor.Position = new Point(15, 7);
        _console.Cursor.IsVisible = true;
        _console.Cursor.MouseClickReposition = true;

        _console.SadComponents.Add(new RandomBackgroundKeyboardComponent());
        _console.MouseMove += OnMouseMove!;
        _console.MouseExit += OnMouseExit!;
        _console.MouseEnter += OnMouseEnter!;

        var c2 = new Console(30, 15);
        c2.Position = new Point(28, 13);
        c2.DefaultBackground = Color.AnsiCyan;
        c2.Clear();

        Children.Add(c2);
        Children.Add(_console);

        void OnMouseEnter(object sender, MouseScreenObjectState mouseState)
        {
            if (sender is Console c)
            {
                c.Cursor.IsVisible = true;
                c.Cursor.Position = new Point(15, 7);
            }
        }

        void OnMouseExit(object sender, MouseScreenObjectState mouseState)
        {
            if (sender is Console c)
            {
                c.Cursor.IsVisible = false;
                c.Clear();
            }
        }

        void OnMouseMove(object sender, MouseScreenObjectState mouseState)
        {
            if (sender is Console c)
            {
                c.Print(1, 1, $"Mouse position: {mouseState.CellPosition}  ");
                if (mouseState.Mouse.LeftButtonDown)
                    c.Print(1, 2, $"Left button is down");
                else
                    c.Print(1, 2, $"                   ");
            }
        }
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Container)
            _console.IsFocused = true;
        base.OnParentChanged(oldParent, newParent);
    }
}