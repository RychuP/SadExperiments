namespace SadExperiments.MainScreen;

/// <summary>
/// Smaller, named <see cref="ScreenSurface"/> that can be added to a page (accessible with the property page.SubPage).
/// </summary>
internal class SubPage : ScreenSurface
{
    public SubPage() : this(Program.Width - 2, Program.Height - 2)
    {
        Position = (1, 1);
    }

    public SubPage(int w, int h) : base(w, h)
    {
        Surface.SetDefaultColors(Color.Black, Color.LightBlue);
    }
}