using GoRogue.DiceNotation;
using ShaiRandom.Generators;

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

    /// <summary>
    /// Draws a box around the perimeter of the <see cref="ICellSurface.Area"/>.
    /// </summary>
    /// <param name="fg">Foreground <see cref="Color"/>.</param>
    public static void DrawOutline(this ICellSurface cellSurface, Color? fg = null)
    {
        cellSurface.DrawBox(cellSurface.Area, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
            new ColoredGlyph(fg ?? Color.Pink, Color.Black)));
    }

    /// <summary>
    /// Rolls a d4 dice to select a random cardinal <see cref="Direction"/>.
    /// </summary>
    /// <returns>Random cardinal direction.</returns>
    public static Direction RandomCardinalDirection(this IEnhancedRandom rand) =>
        Direction.Up + Dice.Roll("1d4*2");
}