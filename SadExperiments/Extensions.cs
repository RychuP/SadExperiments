namespace SadExperiments;

public static class Extensions
{
    /// <summary>
    /// Prints text centered on the surface.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="y"></param>
    /// <param name="c"></param>
    public static void Print(this ICellSurface s, int y, string t, Color? c = null) =>
        s.Print(0, y, t.Align(HorizontalAlignment.Center, s.Width), c ?? s.DefaultForeground);

    /// <summary>
    /// Allows adding multiple screen objects at the same time.
    /// </summary>
    public static void Add(this ScreenObjectCollection collection, params IScreenObject[] childrenList) =>
        Array.ForEach(childrenList, child => collection.Add(child));

    /// <summary>
    /// Sets the default <see cref="ICellSurface"/> colors and clears it.
    /// </summary>
    /// <param name="fg"><see cref="ICellSurface.DefaultForeground"/> color.</param>
    /// <param name="bg"><see cref="ICellSurface.DefaultBackground"/> color.</param>
    /// <param name="clear">True (default) clears the console. False doesn't.</param>
    public static void SetDefaultColors(this ICellSurface cellSurface, Color? fg = null, Color? bg = null, bool clear = true)
    {
        cellSurface.DefaultForeground = fg is null ? Color.White : fg.Value;
        cellSurface.DefaultBackground = bg is null ? Color.Transparent : bg.Value;
        if (clear) cellSurface.Clear();
    }
}