namespace SadExperiments.Pages;

internal class CellSurfaceResizing : Page
{
    public CellSurfaceResizing()
    {
        Title = "Cell Surface Resizing";
        Summary = "Testing referencing the same cell surface, flags and resizing.";

        // test the absolute position
        this.Print(1, 1, this.AbsolutePosition.ToString());
        this.Print(1, 2, this.UsePixelPositioning.ToString());

        // create a test surface
        var newSurface = new ScreenSurface(10, 5) { Position = (20, 1), Parent = this };
        newSurface.Surface.DefaultBackground = Color.LightGreen;
        newSurface.Surface.Clear();

        // another surface that references the same cell surface as the test surface above
        var otherSurface = new ScreenSurface(newSurface.Surface) { Parent = this };
        otherSurface.Position = (1, 5);

        // testing flags
        byte x = 7;
        otherSurface.Surface.Print(1, 2, Helpers.HasFlag(x, 2).ToString());
        int y = Helpers.UnsetFlag(x, 2);
        otherSurface.Surface.Print(1, 3, Helpers.HasFlag(y, 2).ToString());

        // testing cell surface resizing
        (otherSurface.Surface as CellSurface)?.Resize(15, 10, 10, 10, false);
        otherSurface.Surface.Print(1, 5, otherSurface.Surface.ViewWidth.ToString());

        // resizing with a wipe
        this.Surface.DefaultBackground = Color.Brown;
        (this.Surface as CellSurface)?.Resize(Width - 5, Height - 5, Width - 5, Height - 5, true);

        // print absolute position again
        this.Print(1, 20, this.AbsolutePosition.ToString());
        this.Print(1, 22, this.UsePixelPositioning.ToString());
    }
}