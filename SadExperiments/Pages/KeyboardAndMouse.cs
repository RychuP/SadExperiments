using SadConsole.Components;

namespace SadExperiments.Pages;

internal class KeyboardAndMouse : Page
{
    public KeyboardAndMouse()
    {
        Title = "Keyboard And Mouse";
        Summary = "Input handling via components or events.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Input, Tag.Keyboard, Tag.Mouse, Tag.IComponent };

        (int Y, string Text)[] prompts = {
            (2, "Press 'Space' to change color."),
            (4, "Hover mouse over consoles"),
            (5, "to display position."),
            (7, "Click to reposition cursor."),
            (9, "Typing is turned off.")
        };
        Array.ForEach(prompts, p => Surface.Print(45, p.Y, p.Text));

        // create top left console
        var c1 = new Console(30, 15)
        {
            Position = new Point(10, 1),
            DefaultBackground = Color.AnsiGreen
        };
        c1.Clear();
        c1.Cursor.Position = new Point(15, 7);
        c1.Cursor.IsVisible = true;
        c1.Cursor.MouseClickReposition = true;

        // add keyboard and mouse handling
        SadComponents.Add(new RandomBackgroundKeyboardComponent(c1));
        c1.MouseMove += OnMouseMove!;
        c1.MouseExit += OnMouseExit!;
        c1.MouseEnter += OnMouseEnter!;

        // create bottom right console
        var c2 = new Console(30, 15)
        {
            Position = new Point(28, 13),
            DefaultBackground = Color.AnsiCyan
        };
        c2.Clear();

        // add both consoles to Children
        Children.Add(c2, c1);
    }

    void OnMouseEnter(object sender, MouseScreenObjectState mouseState)
    {
        if (sender is Console c)
        {
            c.Cursor.IsVisible = true;
            c.Cursor.Position = c.Area.Center;
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

    class RandomBackgroundKeyboardComponent : KeyboardConsoleComponent
    {
        readonly Console _console;

        public RandomBackgroundKeyboardComponent(Console console) =>
            _console = console;

        public override void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
        {
            handled = false;

            if (keyboard.HasKeysPressed && keyboard.IsKeyPressed(Keys.Space))
            {
                _console.DefaultBackground = Program.RandomColor;
                _console.Clear();
                handled = true;
            }
        }
    }
}