using GoRogue.DiceNotation;
using ShaiRandom.Generators;

namespace SadExperiments;

public static class ICellSurfaceExtensions
{
    /// <summary>
    /// Prints text centered on the surface.
    /// </summary>
    /// <param name="text">Text to print.</param>
    /// <param name="y">Y coordinate.</param>
    /// <param name="color">Foreground <see cref="Color"/>.</param>
    public static void Print(this ICellSurface cellSurface, int y, string text, Color? color = null) =>
        cellSurface.Print(0, y, text.Align(HorizontalAlignment.Center, cellSurface.Width), color ?? cellSurface.DefaultForeground);

    public static void Print(this ICellSurface cellSurface, int y, ColoredString text)
    {
        int x = (cellSurface.Width - text.Length) / 2;
        cellSurface.Print(x, y, text);
    }

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
    /// Draws a rectangle around the perimeter of the <see cref="ICellSurface.Area"/>.
    /// </summary>
    /// <param name="fg">Foreground <see cref="Color"/>.</param>
    /// <param name="glyph">Glyph to use as an outline.</param>
    public static void DrawOutline(this ICellSurface cellSurface, Color? fg = null, int? glyph = null) =>
        cellSurface.DrawRectangle(cellSurface.Area, fg, glyph);

    /// <summary>
    /// Draws a rectangle outline using either <see cref="ICellSurface.ConnectedLineThin"/> or the given glyph.
    /// </summary>
    /// <param name="rectangle"><see cref="Rectangle"/> to draw.</param>
    /// <param name="glyph">Glyph to use as an outline.</param>
    /// <param name="fg">Foreground <see cref="Color"/>.</param>
    public static void DrawRectangle(this ICellSurface cellSurface, Rectangle rectangle, Color? fg = null, int? glyph = null)
    {
        var style = (glyph.HasValue) ?
            ShapeParameters.CreateStyledBox(ICellSurface.CreateLine(glyph.Value),
                new ColoredGlyph(fg ?? Color.White, Color.Transparent)) :
            ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
                new ColoredGlyph(fg ?? Color.White, Color.Transparent));
        cellSurface.DrawBox(rectangle, style);
    }

    /// <summary>
    /// Draws a <see cref="ColoredGlyph"/> at the specified location.
    /// </summary>
    /// <param name="position">Position on the surface.</param>
    /// <param name="glyph"><see cref="ColoredGlyph"/> to draw.</param>
    public static void SetGlyph(this ICellSurface cellSurface, Point position, ColoredGlyph glyph) =>
        cellSurface.SetGlyph(position.X, position.Y, glyph);
}

public static class DirectionExtensions
{
    /// <summary>
    /// Rolls a d4 dice to select a random cardinal <see cref="Direction"/>.
    /// </summary>
    /// <returns>Random cardinal direction.</returns>
    public static Direction NextCardinalDirection(this IEnhancedRandom rand) =>
        Direction.Up + Dice.Roll("1d4*2");

    /// <summary>
    /// Inverses direction: left will be right, up will be down, etc.
    /// </summary>
    /// <param name="direction"><see cref="Direction"/> to inverse.</param>
    /// <returns>Inversed <see cref="Direction"/>.</returns>
    public static Direction Inverse(this Direction direction) =>
        direction + 4;
}

public static class Extensions
{
    /// <summary>
    /// Allows adding multiple screen objects at the same time.
    /// </summary>
    public static void Add(this ScreenObjectCollection collection, params IScreenObject[] childrenList) =>
        Array.ForEach(childrenList, child => collection.Add(child));
}