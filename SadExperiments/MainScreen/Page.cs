namespace SadExperiments.MainScreen;

/// <summary>
/// Page with some bite size SadConsole related content.
/// </summary>
class Page : Console
{
    public string Title { get; init; } = string.Empty;

    public string Summary { get; init; } = string.Empty;
    
    public Tag[] Tags { get; init; } = Array.Empty<Tag>();

    /// <summary>
    /// Submitter of the page to the repository.
    /// </summary>
    public Submitter Submitter { get; init; }

    /// <summary>
    /// Date page was added.
    /// </summary>
    public DateOnly Date { get; init; }

    public int Index { get; set; }

    // default constructor
    public Page() : this(Program.Width, Program.Height) { }

    // constructor that takes width and height
    public Page(int w, int h) : base(Program.Width, Program.Height, w, h)
    {
        UsePixelPositioning = true;
        Position = (0, Header.MinimizedViewHeight * GameHost.Instance.DefaultFont.GlyphHeight);
    }

    /// <summary>
    /// Named <see cref="ScreenSurface"/> at 0 index that can be used for some quick content highlighting.
    /// </summary>
    public SubPage SubPage
    {
        get => Children[0] as SubPage ?? throw new Exception("There is no SubPage added to the Children of this Page.");
        set => Children[0] = value;
    }

    /// <summary>
    /// Adds a <see cref="ScreenSurface"/> centered (pixel positioning) to the page.
    /// </summary>
    /// <param name="child"><see cref="ScreenSurface"/> to add.</param>
    public void AddCentered(ScreenSurface child)
    {
        Children.Add(child);
        child.UsePixelPositioning = true;

        int childWidth = child.WidthPixels;
        int x = WidthPixels / 2 - childWidth / 2;

        int childHeight = child.HeightPixels;
        int y = HeightPixels / 2 - childHeight / 2;
        y = y < 0 ? 0 : y;

        child.Position = (x, y);
    }
}

/// <summary>
/// Indicates that the page implementing this interface can restart its content.
/// </summary>
internal interface IRestartable
{
    void Restart();
}