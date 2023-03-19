using SadConsole.Components;
using SadExperiments;

namespace SadExperiments.Pages;

internal class FocusSetting : Page
{
    public FocusSetting()
    {
        Title = "Focus Setting";
        Summary = "Test of the IsFocused property of consoles.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Keyboard, Tag.Mouse, Tag.Focus, Tag.IComponent };

        var a = new ClickableConsole();
        var b = new ClickableConsole();

        // create a container for a and b
        int childrenWidth = a.Width + b.Width;
        int gap = (Width - childrenWidth) / 3;
        var container = new ScreenSurface(childrenWidth + gap, a.Height);
        container.Children.Add(a);
        container.Children.Add(b);

        // shift b to the right
        b.Position = (container.Surface.Width - b.Width, 0);

        AddCentered(container);

        Surface.Print(4, ColoredString.Parser.Parse("Only one [c:r f:lightgreen]ScreenObject[c:undo] at a time can hold keyboard focus."));
        Surface.Print(6, "Click the below consoles and notice how their focus changes.");
    }

    class ClickableConsole : Console
    {
        public ClickableConsole() : base(20, 3)
        {
            Surface.SetDefaultColors(Color.Black, Color.LightBlue);
            FocusOnMouseClick = true;

            // just playing with components
            SadComponents.Add(new ShowFocus());
            SadComponents.Add(new KeyboardHandler());
        }
    }

    class ShowFocus : UpdateComponent
    {
        public override void Update(IScreenObject host, TimeSpan delta)
        {
            if (host is Console c)
            {
                c.Clear();
                c.Print(1, 1, $"IsFocused: {c.IsFocused}");
            }
        }
    }

    class KeyboardHandler : KeyboardConsoleComponent
    {
        public override void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
        {
            handled = host is Console c && c.Parent.Parent.ProcessKeyboard(keyboard);
        }
    }
}
