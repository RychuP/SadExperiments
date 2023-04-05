namespace SadExperiments.Games.RogueLike.Screens;

internal abstract class PanelWithSeparator : ScreenSurface
{
    protected const int Padding = 2;
    public PanelWithSeparator(int w, int h) : base(w, h)
    {
        Surface.SetDefaultColors(Colors.PanelFG, Colors.PanelBG);
        var separator = new ScreenSurface(1, h);
        separator.Surface.DrawLine(Point.Zero, (0, h - 1), '|', Color.LightSlateGray);
        Children.Add(separator);
    }
}