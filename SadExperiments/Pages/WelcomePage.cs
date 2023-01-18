namespace SadExperiments.Pages;

internal class WelcomePage : Page, IRestartable
{
    readonly AnimatedScreenSurface _worm;

    public WelcomePage()
    {
        Title = "Welcome Page";
        Summary = "Experiments with various features of SadConsole and related libraries.";

        Surface.Print(3, "Press F1 or F2 to navigate between screens.");
        Surface.Print(5, "Press F3 to display the list of contents.");
        Surface.Print(7, "Use arrow keys, space button, etc to interact with individual pages.");
        Surface.Print(9, "For reference, page counter is in the top right corner.");
        Surface.Print(13, "Take everything with a pinch of salt.");
        Surface.Print(15, "These are my own attempts at learning the library");
        Surface.Print(17, "not necessarily examples of the best practice.");

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