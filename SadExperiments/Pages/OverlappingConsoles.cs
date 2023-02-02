using SadConsole.Components;
using SadConsole.Quick;

namespace SadExperiments.Pages;

internal class OverlappingConsoles : Page
{
    public OverlappingConsoles()
    {
        Title = "Overlapping Consoles";
        Summary = "Following Thraka's tutorials: part 2, parenting.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Cursor, Tag.ScreenSurface, Tag.Input, Tag.Keyboard, Tag.Mouse, Tag.Focus };

        // print prompts at the bottom of the page
        int y = Height - 5; 
        Surface.Print(y, "Click to move consoles to the top and set focus.");
        ColoredString t = ColoredString.Parser.Parse("[c:r f:LightGreen]MouseClickReposition[c:undo] is true on the blue console.");
        int x = Width / 2 - t.Length / 2;
        Surface.Print(x, y + 2, t);

        // create top left console
        var c1 = new CursorExample(Color.AnsiCyan, (3, 2), 60, 14);
        c1.Cursor.MouseClickReposition = true;

        // add handling of the page controls
        c1.WithKeyboard((o, k) => ProcessKeyboard(k));

        // add random garbage square to the center of the first console
        ScreenSurface squareWithGarbage = new(5, 3) { UseMouse = false };
        squareWithGarbage.Position = c1.Area.Center - (squareWithGarbage.Surface.Area.Size / 2);
        squareWithGarbage.Surface.FillWithRandomGarbage(squareWithGarbage.Font);
        c1.Children.Add(squareWithGarbage);

        // create bottom right console
        var c2 = new CursorExample(Color.AnsiRed, (19, 11), 58, 12);

        // add handling of the page controls
        c2.WithKeyboard((o, k) => ProcessKeyboard(k));

        // add both consoles to Children (order matters)
        Children.Add(c2, c1);
    }

    // set focus to one of the child consoles when this page becomes active
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

    class CursorExample : Console
    {
        public CursorExample(Color bg, Point position, int w, int h) : base(w, h) 
        {
            Position = position;

            // change background color
            DefaultBackground = bg;
            Surface.Clear();

            // print prompt
            Surface.Print(1, 1, "Type on me!");

            // mouse handling
            FocusOnMouseClick = true;
            MoveToFrontOnMouseClick = true;

            // cursor config
            Cursor.Position = new Point(1, 2);
            Cursor.IsEnabled = true;
            Cursor.IsVisible = true;
        }
    }
}