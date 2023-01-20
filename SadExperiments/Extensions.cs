namespace SadExperiments;

public static class Extensions
{
    /// <summary>
    /// Prints text centered on the surface.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="t"></param>
    /// <param name="y"></param>
    /// <param name="c"></param>
    public static void Print(this ICellSurface s, int y, string t, Color? c = null) =>
        s.Print(0, y, t.Align(HorizontalAlignment.Center, s.Width), c ?? s.DefaultForeground);

    public static void Add(this ScreenObjectCollection collection, params IScreenObject[] childrenList) =>
        Array.ForEach(childrenList, child => collection.Add(child));

    public static void SetDefaultColors(this IScreenSurface screenSurface, Color fg, Color bg)
    {
        screenSurface.Surface.DefaultForeground = fg;
        screenSurface.Surface.DefaultBackground = bg;
        screenSurface.Surface.Clear();
    }
}