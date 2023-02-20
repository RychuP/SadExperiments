using SadCanvas;
using SadConsole.Instructions;
using System.IO;

namespace SadExperiments.Games.Tetris;

class Display : ScreenSurface
{
    protected Canvas? _image;
    public Display(int w, int h) : base(w, h) { }
    protected void ShowImage(string imageFileName)
    {
        var filePath = Path.Combine("Resources", "Images", "Tetris", imageFileName);
        _image = new Canvas(filePath)
        {
            Parent = this,
            UsePixelPositioning = true,
            
        };
        _image.Position = ((WidthPixels - _image.Width) / 2, 0);
    }
}

class InfoDisplay : ScreenSurface
{
    readonly Point _hiddenPosition = (0, -8);
    readonly ScoreDisplay _scoreDisplay;
    readonly LevelDisplay _levelDisplay;

    public InfoDisplay(int w) : base(w, 8)
    {
        UsePixelPositioning = true;

        _scoreDisplay = new ScoreDisplay(w);
        _levelDisplay = new LevelDisplay(w) { Position = _hiddenPosition };
        Children.Add(_levelDisplay, _scoreDisplay);
    }

    public void ShowScore(int score)
    {
        if (_scoreDisplay.Position != Point.Zero)
        {
            _scoreDisplay.Position = Point.Zero;
            _levelDisplay.Position = _hiddenPosition;
        }
        _scoreDisplay.ShowScore(score);
    }

    public void RemoveInstructions() => SadComponents.Clear();

    public void ShowLevel(int level)
    {
        _levelDisplay.ShowLevel(level);

        var instructions = new InstructionSet() { RemoveOnFinished = true }
            .Code((o, t) =>
            {
                _levelDisplay.Position = Point.Zero;
                _scoreDisplay.Position = _hiddenPosition;
                return true;
            })
            .Code((o, t) =>
            {
                Surface.SetDefaultColors(Color.Black, Color.LightGray);
                return true;
            })
            .Code((o, t) =>
            {
                Surface.SetDefaultColors(Color.White, Color.Black);
                return true;
            })
            .Wait(TimeSpan.FromSeconds(1.2))
            .Code((o, t) =>
            {
                Surface.SetDefaultColors(Color.Black, Color.LightGray);
                return true;
            })
            .Code((o, t) =>
            {
                Surface.SetDefaultColors(Color.White, Color.Black);
                return true;
            })
            .Code((o, t) =>
            {
                _levelDisplay.Position = _hiddenPosition;
                _scoreDisplay.Position = Point.Zero;
                return true;
            });
        SadComponents.Add(instructions);
    }
}

class NextDisplay : Display
{
    readonly TetrominoDisplay _tetrominoDisplay = new();

    public NextDisplay(int w) : base(w, 9)
    {
        Surface.SetDefaultColors(Color.White, Color.Black);
        UsePixelPositioning = true;
        ShowImage("next.png");
        Children.Add(_tetrominoDisplay);
        _tetrominoDisplay.Position = (0, Convert.ToInt32(_image!.Height * 1.5));
    }

    public void ShowNext(Tetromino t) =>
        _tetrominoDisplay.ShowTetromino(t);
}

class TetrominoDisplay : ScreenSurface
{
    public TetrominoDisplay() : base(1, 2)
    {
        Font = Fonts.Square10;
        FontSize *= 2.5;
        UsePixelPositioning = true;
    }

    public void ShowTetromino(Tetromino t)
    {
        if (t.Area.Width != Surface.Width && Surface is ICellSurfaceResize sr)
        {
            sr.Resize(t.Area.Width, Surface.Height, true);
            if (Parent is ScreenSurface parent)
                Position = ((parent.WidthPixels - WidthPixels) / 2, Position.Y);
        }
        else
            Surface.Clear();

        foreach (var block in t.Blocks)
        {
            Point delta = t.Type is Tetromino.Shape.O ? (5, 0) : (4, 0);
            (int x, int y) = block.Position - delta;
            Surface.SetCellAppearance(x, y, block.Appearance);
        }
    }
}

class ScoreDisplay : StatDisplay
{
    public ScoreDisplay(int w) : base(w, "score.png", Color.White) { }
    public void ShowScore(int score) => Print(score);
}

class LevelDisplay : StatDisplay
{
    public LevelDisplay(int w) : base(w, "level.png", Color.LightGreen) { }
    public void ShowLevel(int level) => Print(level);
}

class StatDisplay : Display
{
    readonly NumberDisplay _numberDisplay;
    public StatDisplay(int w, string imageFileName, Color textColor) : base(w, 8)
    {
        ShowImage(imageFileName);
        _numberDisplay = new(textColor);
        _numberDisplay.Position = ((WidthPixels - _numberDisplay.WidthPixels) / 2, _image!.Height + 20);
        _numberDisplay.ShowNumber(0);
        Children.Add(_numberDisplay);
    }
    protected void Print(int number) => _numberDisplay.ShowNumber(number);
}

class NumberDisplay : ScreenSurface
{
    readonly Color _textColor;

    public NumberDisplay(Color textColor) : base (1, 1)
    {
        UsePixelPositioning = true;
        _textColor = textColor;
        FontSize *= 3;
    }

    public void ShowNumber(int number)
    {
        string text = number.ToString();
        if (text.Length != Surface.Width && Surface is ICellSurfaceResize surface)
        {
            surface.Resize(text.Length, 1, true);
            if (Parent is ScreenSurface parent)
                Position = ((parent.WidthPixels - WidthPixels) / 2, Position.Y);
        }
        Surface.Print(0, 0, text, _textColor);
    }
}