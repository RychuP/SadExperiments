using SadConsole.Quick;
using SadExperiments.UI;

namespace SadExperiments.Pages;

internal class RectangleBisection : Page
{
    readonly HorizontalButtonsConsole _buttons;
    Rectangle[] _rectangles = Array.Empty<Rectangle>();
    readonly Rectangle _rectangle;
    string _lastRectangleinfo = string.Empty;

    public RectangleBisection()
    {
        Title = "Rectangle Bisection";
        Summary = "Testing rectangle bisection methods.";

        // create buttons console
        _buttons = new(Width, Height) { Parent = this };
        _buttons.WithMouse((o, m) => ProcessMouse(m));

        // create base rectangle
        _rectangle = new Rectangle(Surface.Area.Center, Width / 2 - 4, Height / 2 - 5);
        _rectangle.DrawOutline(Surface);

        // add bottom row buttons
        int y = Height - 2;
        var button = _buttons.AddButton("BisectHorizontally", y);
        button.Click += (o, e) =>
        {
            var result = _rectangle.BisectHorizontally().ToEnumerable();
            DrawResult(result);
            _rectangles = result.ToArray();
        };
        button.InvokeClick();
        _buttons.AddButton("BisectVertically", y).Click += (o, e) =>
        {
            var result = _rectangle.BisectVertically().ToEnumerable();
            DrawResult(result);
            _rectangles = result.ToArray();
        };

        // add top row buttons
        y = 3;
        for (int i = 2; i < 12; i++)
        {
            _buttons.AddButton(i.ToString(), y).Click += (o, e) =>
            {
                if (o is AutomatedButton cb)
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