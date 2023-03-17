namespace SadExperiments.Games.PacMan;

class Header : ScreenSurface
{
    readonly ScoreDisplay _scoreDisplay;
    readonly ColoredString _livesTitle = GetTitle("Lives");
    readonly ColoredString _levelTitle = GetTitle("Level");
    readonly Point _levelPosition;
    const int Margin = 2;

    public Header() : base(Program.Width / 2, 1)
    {
        FontSize *= 2;
        UsePixelPositioning = true;
        _levelPosition = (Surface.Width - _levelTitle.Length - 2 - Margin, 0);

        // score display surface
        var scoreTitle = GetTitle("Score");
        _scoreDisplay = new(scoreTitle);
        Children.Add(_scoreDisplay);
    } 

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Game game)
            Position = (0, 10);
        base.OnParentChanged(oldParent, newParent);
    }

    static ColoredString GetTitle(string text) =>
        ColoredString.Parser.Parse($"[c:r f:lightgreen]{text}:[c:undo] ");

    public void PrintScore(int score) =>
        _scoreDisplay.Print(score);

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

class ScoreDisplay : ScreenSurface
{
    readonly ColoredString _title;
    public ScoreDisplay(ColoredString title) : base(title.Length + 3, 1)
    {
        FontSize *= 2;
        UsePixelPositioning = true;
        _title = title;
    }

    public void Print(int score)
    {
        ColoredString text = _title + $"{score}";
        if (text.Length != Surface.Width)
            Resize(text.Length);
        Surface.Print(0, 0, text);
    }

    void Resize(int width)
    {
        if (Surface is ICellSurfaceResize cellSurfaceResize)
        {
            cellSurfaceResize.Resize(width, 1, true);
            CenterPosition();
        }
    }

    void CenterPosition()
    {
        if (Parent is ScreenSurface screenSurface)
        {
            int x = (screenSurface.WidthPixels - WidthPixels) / 2;
            Position = (x, 0);
        }
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is ScreenSurface)
            CenterPosition();
        base.OnParentChanged(oldParent, newParent);
    }
}