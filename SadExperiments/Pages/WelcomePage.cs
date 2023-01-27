namespace SadExperiments.Pages;

internal class WelcomePage : Page, IRestartable
{
    readonly AnimatedScreenSurface _worm;

    public WelcomePage()
    {
        Title = "Welcome Page";
        Summary = "Experiments with various features of SadConsole and related libraries.";

        UsePrintProcessor = true;

        Surface.Print(3, "Press F1 or F2 to navigate between screens.");
        Surface.Print(5, "Press F3 to display the list of contents.");
        Surface.Print(7, "Press F4 for Color Picker and F5 for Character Picker windows.");
        Surface.Print(9, "Use arrow keys, space button, etc to interact with individual pages.");
        Surface.Print(13, "Take everything with a pinch of salt.");
        Surface.Print(15, "These are my own attempts at learning the library");
        Surface.Print(17, "not necessarily examples of the best practice.");

        // keyboard shortcut decorations
        for (int i = 0; i < Surface.Count; i++)
        {
            if (Surface[i].Glyph == 'F')
            {
                Surface[i].Foreground = Color.LightGreen;
                Surface[i + 1].Foreground = Color.LightGreen;
                Surface[i + Surface.Width].Glyph = '_';
                Surface[i + Surface.Width].Mirror = Mirror.Vertical;
                Surface[i + Surface.Width].Foreground = Color.LightBlue;
                Surface[i + 1 + Surface.Width].Glyph = '_';
                Surface[i + 1 + Surface.Width].Mirror = Mirror.Vertical;
                Surface[i + 1 + Surface.Width].Foreground = Color.LightBlue;
            }
        }

        // load the animation of the worm
        string path = "./Resources/Images/crawlingworm.png";
        _worm = AnimatedScreenSurface.FromImage("Crawling Worm", path, (1, 9), 0.15f, (0, 1), font: Fonts.ThickSquare8);
        _worm.FontSize *= 0.5;
        _worm.UsePixelPositioning = true;
        _worm.Repeat = true;

        Children.Add(_worm);
        _worm.Start();
        Restart();
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