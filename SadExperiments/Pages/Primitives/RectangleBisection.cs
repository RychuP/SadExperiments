using SadExperiments;
using SadExperiments.UI;
using SadExperiments.UI.Controls;

namespace SadExperiments.Pages;

internal class RectangleBisection : Page
{
    Rectangle[] _rectangles = Array.Empty<Rectangle>();
    readonly Rectangle _rectangle;
    string _lastRectangleinfo = string.Empty;

    public RectangleBisection()
    {
        Title = "Rectangle Bisection";
        Summary = "Testing rectangle bisection methods.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Primitives, Tag.Rectangle, Tag.Input, Tag.Keyboard, Tag.Mouse, Tag.UI };

        // create base rectangle
        _rectangle = new Rectangle(Surface.Area.Center, Width / 2 - 4, Height / 2 - 5);
        Surface.DrawRectangle(_rectangle);

        // create bottom buttons console
        var bottomButtons = new HorizontalButtonsConsole(Width, 1)
        {
            Parent = this,
            Position = new Point(0, Height - 2),
        };

        // add bottom row buttons
        var button = bottomButtons.AddButton("BisectHorizontally");
        button.Click += (o, e) =>
        {
            var result = _rectangle.BisectHorizontally().ToEnumerable();
            DrawResult(result);
            _rectangles = result.ToArray();
        };
        button.InvokeClick();

        // add bottom row buttons
        bottomButtons.AddButton("BisectVertically").Click += (o, e) =>
        {
            var result = _rectangle.BisectVertically().ToEnumerable();
            DrawResult(result);
            _rectangles = result.ToArray();
        };

        // create top buttons console
        var topButtons = new HorizontalButtonsConsole(Width, 1)
        {
            Parent = this,
            Position = new Point(0, 3)
        };

        // add top row buttons
        for (int i = 2; i < 12; i++)
        {
            topButtons.AddButton(i.ToString()).Click += (o, e) =>
            {
                if (o is VariableWidthButton cb)
                {
                    int minDimension = Convert.ToInt32(cb.Text);
                    Surface.Print(1, "BisectRecursive: " + minDimension);
                    var result = _rectangle.BisectRecursive(minDimension);
                    DrawResult(result);
                    _rectangles = result.ToArray();
                }
            };
        }

        Surface.Print(1, "BisectRecursive: X");
    }

    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        if (_rectangle.Contains(state.CellPosition))
        {
            var rectangle = Array.Find(_rectangles, r => r.Contains(state.CellPosition));
            _lastRectangleinfo = $"Rectangle size: {rectangle.Size}";
            Surface.Print(1, 1, _lastRectangleinfo);
        }
        else
            Surface.Print(1, 1, new string(' ', _lastRectangleinfo.Length));
        return base.ProcessMouse(state);
    }

    void DrawResult(IEnumerable<Rectangle> result)
    {
        foreach (var rect in result)
            DrawRectangle(rect);
    }

    void DrawRectangle(Rectangle rect)
    {
        var color = Program.RandomColor;
        foreach (var point in rect.Positions())
            Surface.SetBackground(point.X, point.Y, color);
    }
}