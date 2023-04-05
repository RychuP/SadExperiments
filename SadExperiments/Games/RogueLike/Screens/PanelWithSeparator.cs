namespace SadExperiments.Games.RogueLike.Screens;

internal abstract class PanelWithSeparator : ScreenSurface
{
    protected const int Padding = 2;
    public PanelWithSeparator(int w, int h) : base(w, h)
    {
        Surface.SetDefaultColors(Colors.DefaultFG, Colors.PanelBG);
        var separator = new ScreenSurface(1, h);
        separator.Surface.DrawLine(Point.Zero, (0, h - 1), '|', Colors.PanelSeparator);
        Children.Add(separator);
    }
    protected void Print(int y, string text, Color? color = null)
    {
        Surface.Print(Padding, y, text.Align(HorizontalAlignment.Left, Surface.Width), 
            color ?? Colors.DefaultFG);
    }
}