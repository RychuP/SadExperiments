namespace SadExperiments.MainScreen;

/// <summary>
/// Smaller, named screen surface that can be added to a page. It can be accessed with the property page.SubPage.
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