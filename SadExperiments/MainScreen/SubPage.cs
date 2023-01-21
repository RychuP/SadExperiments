namespace SadExperiments.MainScreen;

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