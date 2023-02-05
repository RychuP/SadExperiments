using SadExperiments;

namespace SadExperiments.Pages.PrimitivesPages;

internal class RectangleManipulation : Page
{
    public RectangleManipulation()
    {
        Title = "Rectangle Manipulation";
        Summary = "Messing around with primitives.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Primitives, Tag.Rectangle, Tag.Drawing };

        ColoredGlyph style = new(Color.Violet, Color.Black, 177);
        Rectangle r = new(2, 2, 10, 5);

        // draw the first box and display some info about it
        var box = ICellSurface.ConnectedLineThick;
        box[0] = '/';
        box[1] = '=';
        box[2] = '\\';
        box[6] = box[2];
        box[7] = box[1];
        box[8] = box[0];
        this.DrawBox(r, ShapeParameters.CreateStyledBox(box, style));
        this.Print(2, 10, "Original rectangle position and size:");
        this.Print(2, 11, r.Position.ToString());
        this.Print(2, 12, r.Size.ToString());

        // expand the rectangle by 1 on each side;
        var r2 = r.Expand(1, 1);

        // print some info about the expanded box
        this.Print(2, 14, "Expanded (by 1 on each side) rectangle position and size:");
        this.Print(2, 15, r2.Position.ToString());
        this.Print(2, 16, r2.Size.ToString());

        // move the expanded box right and draw
        style.Foreground = Color.Yellow;
        var r3 = r2.ChangeX(15);
        this.DrawBox(r3, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, style));
    }
}