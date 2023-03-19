namespace SadExperiments.Pages;

internal class WelcomePageOld : Page, IRestartable
{
    readonly AnimatedScreenSurface _worm;

    public WelcomePageOld()
    {
        Title = "Welcome Page";
        Summary = "Experiments with various features of SadConsole and related libraries.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Animations, Tag.Decorators, Tag.ImageConversion };

        int currentRow = PrintPrompts(2, new string[] {
            "Press F1 or F2 to navigate between screens.",
            "Press F3 to display the list of contents.",
            "Press F4 for Color Picker and F5 for Character Picker windows.",
            "Use arrow keys, space button, etc to interact with individual pages."
        }, Color.White);

        PrintPrompts(currentRow + 4, new string[]
        {
            "Take everything with a pinch of salt.",
            "These are my own attempts at learning the library",
            "not necessarily examples of the best practice."
        }, Color.BurlyWood);

        // keyboard shortcut decorations
        for (int i = 0; i < Surface.Count; i++)
        {
            if (Surface[i].Glyph == 'F')
            {
                DecorateLetter(i);
                DecorateLetter(i + 1);
            }
        }

        // load the animation of the worm
        string path = "./Resources/Images/crawlingworm.png";
        _worm = AnimatedScreenSurface.FromImage("Crawling Worm", path, (1, 9), 0.15f, (0, 1), font: Fonts.ThickSquare8);
        _worm.FontSize *= 0.5;
        _worm.UsePixelPositioning = true;
        _worm.Repeat = true;

        // start the animation
        Children.Add(_worm);
        _worm.Start();
        Restart();
    }

    // prints centered prompts starting at the given row
    int PrintPrompts(int row, string[] prompts, Color color)
    {
        int spacer = 2;
        row -= spacer;
        Array.ForEach(prompts, t => Surface.Print(row += spacer, t, color));
        return row;
    }

    // changes the color of the letter at Surface[index] and adds underline
    void DecorateLetter(int index)
    {
        Surface[index].Foreground = Color.LightGreen;
        var underline = new ColoredGlyph(Color.LightBlue, Color.Transparent, '_', Mirror.Vertical);
        var position = Point.FromIndex(index + Width, Width);
        Surface.SetGlyph(position, underline);
    }

    public void Restart()
    {
        _worm.Position = (WidthPixels, _worm.Font.GlyphHeight * 40);
    }

    public override void Update(TimeSpan delta)
    {
        if (_worm.CurrentFrameIndex != 0 || _worm.CurrentFrameIndex == _worm.FrameCount - 1)
            _worm.Position -= (1, 0);

        if (_worm.Position.X + _worm.WidthPixels <= 0)
            _worm.Position = _worm.Position.WithX(WidthPixels);
        
        base.Update(delta);
    }
}