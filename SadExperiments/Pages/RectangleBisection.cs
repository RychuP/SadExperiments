using SadConsole.Quick;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;

namespace SadExperiments.Pages;

internal class RectangleBisection : Page
{
    readonly Buttons _buttons;
    Rectangle[] _rectangles = Array.Empty<Rectangle>();
    Rectangle _rectangle;
    string _lastRectangleinfo = string.Empty;

    public RectangleBisection()
    {
        Title = "Rectangle Bisection";
        Summary = "Testing rectangle bisection methods.";

        // create buttons console
        _buttons = new(Width, Height) { Parent = this };
        _buttons.WithMouse((so, ms) => ProcessMouse(ms));

        // create base rectangle
        _rectangle = new Rectangle(Surface.Area.Center, Width / 2 - 4, Height / 2 - 5);
        var style = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
            new ColoredGlyph(Color.White, Color.Transparent));
        Surface.DrawBox(_rectangle, style);

        // add bottom row buttons
        int rowNumber = Height - 2;
        var button = _buttons.AddButton("BisectHorizontally", rowNumber);
        button.Click += (o, e) =>
        {
            var result = _rectangle.BisectHorizontally().ToEnumerable();
            DrawResult(result);
            _rectangles = result.ToArray();
        };
        button.InvokeClick();
        button = _buttons.AddButton("BisectVertically", rowNumber);
        button.Click += (o, e) =>
        {
            var result = _rectangle.BisectVertically().ToEnumerable();
            DrawResult(result);
            _rectangles = result.ToArray();
        };

        // add top row buttons
        rowNumber = 3;
        for (int i = 2; i < 12; i++)
        {
            _buttons.AddButton(i.ToString(), rowNumber).Click += (o, e) =>
            {
                if (o is CustomButton cb)
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

    class Buttons : ControlsConsole
    {
        public Buttons(int w, int h) : base(w, h)
        {
            DefaultBackground = Color.Transparent;
            Surface.Clear();
        }

        public CustomButton AddButton(string label, int rowNumber)
        {
            var button = new CustomButton(label, rowNumber);
            Controls.Add(button);

            // compute space between buttons for the row
            var buttons = Controls.Where(control => control.Position.Y == rowNumber).ToArray();
            int allButtonsInRowWidth = buttons.Sum(control => control.Width);
            int buttonCount = buttons.Count();
            double spacer = (double)(Width - allButtonsInRowWidth) / (buttonCount + 1);
            if (spacer < 0) spacer = 0;

            // apply new positions
            double x = spacer;
            for (int i = 0; i < buttonCount; i++)
            {
                var control = buttons[i];
                control.Position = control.Position.WithX((int)Math.Round(x));
                x += control.Width + spacer;
            }

            return button;
        }
    }

    class CustomButton : Button
    {
        const int Padding = 4;

        static CustomButton() =>
            Library.Default.SetControlTheme(typeof(CustomButton), new ButtonTheme());

        public CustomButton(string label, int rowNumber) : base(label.Length + Padding, 1)
        {
            Text = label;
            Position = (0, rowNumber);
            UseMouse = true;
            UseKeyboard = false;
        }
    }
}