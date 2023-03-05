namespace SadExperiments.Games.PacMan;

class Score : ScreenSurface
{
    readonly ColoredString _scoreTitle = ColoredString.Parser.Parse("[c:r f:lightgreen]Score:[c:undo] ");

    public Score() : base(20, 1)
    {
        UsePixelPositioning = true;
        FontSize *= 2;
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Game game)
            Position = ((game.WidthPixels - WidthPixels) / 2, 10);
        base.OnParentChanged(oldParent, newParent);
    }

    public void PrintScore(int score)
    {
        Surface.Print(0, _scoreTitle + $"{score:000}");
    }
}