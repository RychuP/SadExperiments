namespace SadExperiments.Games.PacMan;

class Header : ScreenSurface
{
    readonly ColoredString _scoreTitle = GetTitle("Score");
    readonly ColoredString _livesTitle = GetTitle("Lives");
    readonly ColoredString _levelTitle = GetTitle("Level");
    readonly Point _scorePosition;
    readonly Point _levelPosition;
    const int Margin = 2;

    public Header() : base(Program.Width / 2, 1)
    {
        FontSize *= 2;
        UsePixelPositioning = true;
        _scorePosition = ((Surface.Width - _scoreTitle.Length - 3) / 2, 0);
        _levelPosition = (Surface.Width - _levelTitle.Length - 2 - Margin, 0);
    } 

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Game game)
            Position = (0, 10);
        base.OnParentChanged(oldParent, newParent);
    }

    static ColoredString GetTitle(string text) =>
        ColoredString.Parser.Parse($"[c:r f:lightgreen]{text}:[c:undo] ");

    public void PrintScore(int score)
    {
        Surface.Print(_scorePosition, _scoreTitle + $"{score:000}");
    }

    public void PrintLives(int lives)
    {
        Surface.Print(Margin, 0, _livesTitle + $"{lives:00}");
    }

    public void PrintLevel(int level)
    {
        Surface.Print(_levelPosition, _levelTitle + $"{level:00}");
    }

    public void Print(int lives, int score, int level)
    {
        PrintLives(lives);
        PrintScore(score);
        PrintLevel(level);
    }
}