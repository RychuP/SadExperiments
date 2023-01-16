namespace SadExperiments.Pages;

// http://sadconsole.com/v9/articles/tutorials/getting-started/part-1-drawing.html
internal class BasicDrawing : Page
{
    public BasicDrawing()
    {
        Title = "Basic Drawing";
        Summary = "Following Thraka's tutorials: part 1, drawing.";

        var glyph = new ColoredGlyph(Color.Violet, Color.Black, 177);
        var standardGlyph = new ColoredGlyph(Color.White, Color.Black);
        Rectangle rectangle = new(3, 3, 23, 3);
        Rectangle rectangle2 = new(3, 7, 23, 3);
        Rectangle rectangle3 = new(3, 11, 23, 3);

        this.Fill(rectangle, Color.Violet, Color.Black, 0);
        this.Print(4, 4, "Hello from SadConsole", Mirror.None);

        // simple box
        this.DrawBox(rectangle, ShapeParameters.CreateBorder(glyph));

        // thin line border box
        this.DrawBox(rectangle2, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, glyph));

        // thick line border box
        this.DrawBox(rectangle3, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThick, glyph));

        // circle 
        this.DrawCircle(new Rectangle(28, 5, 16, 10), ShapeParameters.CreateFilled(glyph, standardGlyph));

        // line 
        this.DrawLine(new Point(60, 5), new Point(66, 20), '$', Color.AnsiBlue, Color.AnsiBlueBright);

        // manipulate glyphs
        this.SetForeground(15, 4, Color.DarkGreen);
        this.SetBackground(18, 4, Color.DarkCyan);
        this.SetGlyph(4, 4, 64);
        this.SetMirror(10, 4, Mirror.Vertical);
    }
}