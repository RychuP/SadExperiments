using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using System.Reflection.Emit;

namespace SadExperiments.Pages;

internal class RectangleBisection : Page
{
    readonly Buttons _buttons;

    public RectangleBisection()
    {
        Title = "Primitives";
        Summary = "Testing primitives.";

        // create buttons console
        _buttons = new(Width, Height) { Parent = this };

        // create base rectangle
        var rectangle = new Rectangle(Surface.Area.Center, Width / 2 - 4, Height / 2 - 5);
        var style = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
            new ColoredGlyph(Color.White, Color.Transparent));
        Surface.DrawBox(rectangle, style);

        // add bottom row buttons
        int rowNumber = Height - 2;
        var button = _buttons.AddButton("BisectHorizontally", rowNumber);
        button.Click += (o, e) =>
        {
            var result = rectangle.BisectHorizontally();
            DrawResult(result.ToEnumerable());
        };
        button.InvokeClick();
        button = _buttons.AddButton("BisectVertically", rowNumber);
        button.Click += (o, e) =>
        {
            var result = rectangle.BisectVertically();
            DrawResult(result.ToEnumerable());
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
                    var result = rectangle.BisectRecursive(minDimension);
                    DrawResult(result);
                }
            };
        }

        Surface.Print(1, "BisectRecursive: X");
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