using SadExperiments;

namespace SadExperiments.Pages;

class MoveAndResize : Page
{
    const int defaultRowForPrinting = 2;
    const int defaultColumnForPrinting = 3;
    int _currentRow;
    int[] _rowCharCounts = Array.Empty<int>();
    bool _initialized = false;

    enum ActionType
    {
        ConsoleMove,
        ViewMove,
        ViewResize,
        InitialDraw
    }

    public MoveAndResize() : base(Program.Width * 2, Program.Height * 2)
    {
        Title = "Move And Resize";
        Summary = "Exercise in moving and resizing a console and its view with arrow keys.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Primitives, Tag.Input, Tag.Keyboard, Tag.Resizing, Tag.Rectangle, Tag.Color };

        DefaultBackground = Color.AnsiBlackBright;
        UsePixelPositioning = false;
        Position = (1, Header.MinimizedViewHeight + 1);

        // change view
        View = new Rectangle(0, 0, 35, 11);

        ResetRowCounter();

        // print instructions
        PrintInfo("Use arrow keys to move view.");
        _currentRow++;
        PrintInfo("Add left shift to resize view.");
        _currentRow++;
        PrintInfo("Add left ctrl to move console.");
        _currentRow++;
        PrintInfo("Press 'Enter' to start.");
    }

    void ResetRowCounter()
    {
        _currentRow = 0;
        _rowCharCounts = new int[10];
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (_initialized)
        {
            if (keyboard.HasKeysDown)
            {
                // move console
                if (keyboard.IsKeyDown(Keys.LeftControl))
                {
                    if (keyboard.IsKeyDown(Keys.Up))
                    {
                        MoveConsole(Direction.Up);
                    }
                    else if (keyboard.IsKeyDown(Keys.Down))
                    {
                        MoveConsole(Direction.Down);
                    }
                    if (keyboard.IsKeyDown(Keys.Left))
                    {
                        MoveConsole(Direction.Left);
                    }
                    else if (keyboard.IsKeyDown(Keys.Right))
                    {
                        MoveConsole(Direction.Right);
                    }
                }

                // resize view
                else if (keyboard.IsKeyDown(Keys.LeftShift))
                {
                    if (keyboard.IsKeyDown(Keys.Up))
                    {
                        ResizeView(Direction.Up);
                    }
                    else if (keyboard.IsKeyDown(Keys.Down))
                    {
                        ResizeView(Direction.Down);
                    }

                    if (keyboard.IsKeyDown(Keys.Left))
                    {
                        ResizeView(Direction.Left);
                    }
                    else if (keyboard.IsKeyDown(Keys.Right))
                    {
                        ResizeView(Direction.Right);
                    }
                }

                // move view
                else
                {
                    if (keyboard.IsKeyDown(Keys.Up))
                    {
                        MoveView(Direction.Up);
                    }
                    else if (keyboard.IsKeyDown(Keys.Down))
                    {
                        MoveView(Direction.Down);
                    }

                    if (keyboard.IsKeyDown(Keys.Left))
                    {
                        MoveView(Direction.Left);
                    }
                    else if (keyboard.IsKeyDown(Keys.Right))
                    {
                        MoveView(Direction.Right);
                    }
                }
            }
        }

        else
        {
            if (keyboard.HasKeysPressed && keyboard.IsKeyPressed(Keys.Enter))
            {
                // clear the surface
                this.Clear();
                ResetRowCounter();

                // draw borders and basic info about console and view sizes and positions
                DrawNumbers(Width, Height, (x, y, text, color) => this.Print(x, y, text, color));
                DrawNumbers(Height, Width, (y, x, text, color) => this.Print(x, y, text, color));
                PrintStandardInformation(ActionType.InitialDraw);

                // mark this instance as initialized
                _initialized = true;
            }
        }

        return base.ProcessKeyboard(keyboard);
    }

    // draws colored numbers around the perimeter of the console
    static void DrawNumbers(int drawingSideLength, int perpendicularSideLength, Action<int, int, string, Color> Print)
    {
        Color color = Color.White;
        for (int y = 0; y < perpendicularSideLength; y += perpendicularSideLength - 1)
        {
            for (int x = 0; x < drawingSideLength; x++)
            {
                int number = x % 10;
                if (number == 0)
                {
                    color = Program.RandomColor;
                }
                Print(x, y, number.ToString(), color);
            }
        }
    }

    #region Moving And Resizing Logic

    void MoveConsole(Direction d)
    {
        Position += d;
        ErasePreviousInfo();
        PrintStandardInformation(ActionType.ConsoleMove);
    }

    void ResizeView(Direction d)
    {
        ChangeViewRectangle(d.Type, ActionType.ViewResize);
    }

    void MoveView(Direction d)
    {
        ChangeViewRectangle(d.Type, ActionType.ViewMove);
    }

    void ChangeViewRectangle(Direction.Types d, ActionType at)
    {
        // clear previous info
        ErasePreviousInfo();

        // do something with the view rectangle
        Rectangle r = View;
        switch (d)
        {
            case Direction.Types.Up:
                View = at is ActionType.ViewMove ? r.ChangeY(-1) : r.Height > 1 ? r.ChangeHeight(-1) : r;
                break;

            case Direction.Types.Down:
                View = at is ActionType.ViewMove ? r.ChangeY(1) : r.ChangeHeight(1);
                break;

            case Direction.Types.Left:
                View = at is ActionType.ViewMove ? r.ChangeX(-1) : r.Width > 1 ? r.ChangeWidth(-1) : r;
                break;

            case Direction.Types.Right:
                View = at is ActionType.ViewMove ? r.ChangeX(1) : r.ChangeWidth(1);
                break;
        }

        // print current info
        PrintStandardInformation(at);
    }
    #endregion

    #region Information Printing Logic    

    void PrintStandardInformation(ActionType at)
    {
        PrintTitle(at);
        PrintViewInfo();
        PrintConsoleInfo();
    }

    void PrintTitle(ActionType at)
    {
        (string title, Color color) = at switch
        {
            ActionType.ConsoleMove => ("Console has been moved.", Color.Red),
            ActionType.ViewMove => ("View has been moved.", Color.Yellow),
            ActionType.ViewResize => ("View has been resized.", Color.Pink),
            _ => ("Current sizes and coordinates", Color.AnsiCyan)
        };
        PrintInfo(title, color);
        _currentRow++;
    }

    void PrintViewInfo()
    {
        string txt = "View position and size:";
        PrintInfo(txt, View);
    }

    void PrintConsoleInfo()
    {
        string txt = "Console position and size:";
        PrintInfo(txt, new Rectangle(Position.X, Position.Y, Width, Height));
    }

    void PrintInfo(string name, Rectangle r)
    {
        PrintInfo(name, Color.Green);
        string txt = $"{r.Position} {r.Size}";
        PrintInfo(txt);
        _currentRow++;
    }

    void PrintInfo(string name, Color? color = null)
    {
        Color c = color ?? DefaultForeground;

        // calculate coords for printing
        int x = View.X + defaultColumnForPrinting,
            y = View.Y + defaultRowForPrinting + _currentRow;

        // make sure the bottom border doesn't get erased
        if (y < Height - 1)
        {
            // make sure the right border doesn't get erased
            int remainingCellsToRightBorder = Width - x - 1;
            if (name.Length > remainingCellsToRightBorder)
            {
                name = name.Substring(0, remainingCellsToRightBorder);
            }

            // print what remains
            this.Print(x, y, name, c);

            // save string length for future erasing
            _rowCharCounts[_currentRow] = name.Length;

            // move the row counter forward
            _currentRow++;
        }

    }

    void ErasePreviousInfo()
    {
        for (int i = 0; i < _currentRow; i++)
        {
            int x = View.X + defaultColumnForPrinting,
                y = View.Y + defaultRowForPrinting + i;

            // debug
            // this.Print(x, y, new string('x', _rowCharCounts[i]));

            this.Erase(x, y, _rowCharCounts[i]);
        }
        ResetRowCounter();
    }
    #endregion
}