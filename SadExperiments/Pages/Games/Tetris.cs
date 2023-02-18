using Newtonsoft.Json.Linq;
using SadConsole.Components;
using SadConsole.Entities;

namespace SadExperiments.Pages;

class Tetris : Page
{
    public static readonly int SolidGlyph = Fonts.C64.SolidGlyphIndex;

    public TetrisBoard Board;

    public Tetris()
    {
        #region Meta
        Title = "Template";
        Summary = "TemplateSummary";
        Submitter = Submitter.Rychu;
        Date = new(2023, 01, 01);
        Tags = Array.Empty<Tag>();
        #endregion Meta

        var t = new Tetromino();
        Board = new TetrisBoard(t);
        Children.Add(Board);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        Game.Instance.Keyboard.InitialRepeatDelay = newParent is Container ? 0.3f : 
            Container.Instance.DefaultInitialRepeatDelay;
        base.OnParentChanged(oldParent, newParent);
    }

    Tetromino GetRandomTetromino()
    {
        int i = (int)Board.Current.Type + 1;
        if (i == Tetromino.ShapeCount) i = 0;
        return new Tetromino((Tetromino.Shape)i);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed)
        {
            if (keyboard.IsKeyPressed(Keys.X))
                Board.Current.RotateRight();
            else if (keyboard.IsKeyPressed(Keys.Z))
                Board.Current.RotateLeft();
            else if (keyboard.IsKeyPressed(Keys.Left))
                Board.Current.MoveLeft();
            else if (keyboard.IsKeyPressed(Keys.Right))
                Board.Current.MoveRight();
            else if (keyboard.IsKeyPressed(Keys.Space))
            {
                var t = GetRandomTetromino();
                Board.Current = t;
            }
        }
        return base.ProcessKeyboard(keyboard);
    }
}

class TetrisBoard : ScreenSurface
{
    #region Constants

    #endregion Constants

    #region Fields
    readonly TetrominoManager _renderer = new();
    Tetromino _current = new();
    int _gravity = 1;
    const int MaxGravity = 20;
    readonly TimeSpan _frameTime = TimeSpan.FromSeconds(1 / 60);
    Timer _timer;
    #endregion Fields

    #region Constructors
    public TetrisBoard(Tetromino tetromino) : base(12, 22)
    {
        Position = (1, 1);
        Font = Fonts.Square10;
        FontSize *= 2;

        Current = tetromino;

        Surface.DrawOutline(Color.Pink);
        SadComponents.Add(_renderer);

        _timer = new(TimeSpan.FromSeconds(0.5d));
        _timer.TimerElapsed += Timer_OnTimerElapsed;
        SadComponents.Add(_timer);
    }
    #endregion Constructors

    #region Properties
    public Tetromino Current
    {
        get => _current;
        set
        {
            _renderer.Remove(_current);
            _renderer.Add(value);
            _current = value;
        }
    }

    //public Tetromino Next { get; private set; }
    public int Score { get; set; }
    #endregion Properties

    #region Methods
    void Timer_OnTimerElapsed(object? o, EventArgs e)
    {
        Current.MoveDown();
    }
    #endregion Methods

    #region Events
    public event EventHandler? ScoreChanged;
    #endregion Events
}

class TetrominoManager : Renderer
{
    public void Add(Tetromino t)
    {
        foreach (Entity block in t.Blocks)
            Add(block);
    }

    // used for testing
    public void Remove(Tetromino t)
    {
        if (Entities.Count == 0) return;
        foreach (Entity block in t.Blocks)
            Remove(block);
    }
}


class Tetromino
{
    public static readonly int ShapeCount = Enum.GetNames(typeof(Shape)).Length;

    public Shape Type { get; init; }

    public Entity[] Blocks { get; } = new Entity[4];

    public ColoredGlyph Appearance { get; init; }

    readonly int[,] _rotations;
    int _currentRotationIndex = 0;
    Rectangle _area;

    public Tetromino(Shape shape = 0)
    {
        _rotations = shape switch
        {
            Shape.L => new int[,] { { 4, 2, 3, 5 }, { 4, 1, 7, 8 }, { 4, 3, 5, 6 }, { 4, 0, 1, 7 } },
            Shape.J => new int[,] { { 4, 0, 3, 5 }, { 4, 1, 2, 7 }, { 4, 3, 5, 8 }, { 4, 1, 6, 7 } },
            Shape.S => new int[,] { { 4, 1, 2, 3 }, { 4, 1, 5, 8 }, { 4, 5, 6, 7 }, { 4, 0, 3, 7 } },
            Shape.Z => new int[,] { { 4, 0, 1, 5 }, { 4, 2, 5, 7 }, { 4, 3, 7, 8 }, { 4, 1, 3, 6 } },
            Shape.T => new int[,] { { 4, 1, 3, 5 }, { 4, 1, 5, 7 }, { 4, 3, 5, 7 }, { 4, 1, 3, 7 } },
            Shape.O => new int[,] { { 3, 0, 1, 2 }, { 3, 0, 1, 2 }, { 3, 0, 1, 2 }, { 3, 0, 1, 2 } },
                  _ => new int[,] { { 5, 4, 6, 7 }, { 6, 2, 10, 14 }, { 10, 8, 9, 11 }, { 9, 1, 5, 13 } }
        };

        Color fgColor = shape switch
        {
            Shape.L => Color.Orange,
            Shape.J => Color.Blue,
            Shape.S => Color.Green,
            Shape.Z => Color.Red,
            Shape.T => Color.Purple,
            Shape.O => Color.Yellow,
                  _ => Color.Cyan
        };

        int x = 4, y = 5;
        _area = shape switch
        {
            Shape.I => new(x, y, 4, 4),
            Shape.O => new(x + 1, y, 2, 2),
                  _ => new(x, y, 3, 3)
        };

        Type = shape;
        Appearance = new ColoredGlyph(fgColor, Color.Transparent, Tetris.SolidGlyph);

        // create blocks
        for (int i = 0; i < 4; i++)
            Blocks[i] = new Entity(Appearance, 1);

        // assign block positions
        ApplyPositions(0);
    }

    public void MoveDown() => Move(0, 1);
    public void MoveUp() => Move(0, -1);
    public void MoveLeft() => Move(-1, 0);
    public void MoveRight() => Move(1, 0);

    void Move(int dx, int dy)
    {
        foreach (var block in Blocks)
            block.Position = block.Position.Translate(dx, dy);
    }

    public void RotateLeft() => Rotate(-1);
    public void RotateRight() => Rotate(1);

    void Rotate(int direction)
    {
        if (direction == 1 || direction == -1)
        {
            // find the index of the new array of rotations
            int rotationIndex = _currentRotationIndex + direction;
            rotationIndex = rotationIndex < 0 ? 3 :
                            rotationIndex > 3 ? 0 : rotationIndex;

            // recalculate current tetromino area position
            int pivotPosIndex = _rotations[_currentRotationIndex, 0];
            Point pivotLocalPos = Point.FromIndex(pivotPosIndex, _area.Width);
            Point pivotGridPos = Blocks[0].Position;
            _area = _area.WithPosition(pivotGridPos - pivotLocalPos);

            // apply new positions based on the given rotation
            ApplyPositions(rotationIndex);

            // save rotation index
            _currentRotationIndex = rotationIndex;
        }
        else
            throw new ArgumentException("Direction of rotation int outside bounds.");
    }

    // assigns new block positions based on the given rotation
    void ApplyPositions(int rotationIndex)
    {
        for (int i = 0; i < 4; i++)
        {
            int blockPositionIndex = _rotations[rotationIndex, i];
            var deltaPosition = Point.FromIndex(blockPositionIndex, _area.Width);
            Blocks[i].Position = _area.Position + deltaPosition;
        }
    }

    // SRS (Standard Rotation System) https://tetris.fandom.com/wiki/SRS
    //
    // orange
    // L:   @     ..@     .@.     ...     @@.
    //    @X@     @@@     .@.     @@@     .@.
    //            ...     .@@     @..     .@.
    //
    // blue
    // J: @       @..     .@@     ...     .@.
    //    @X@     @@@     .@.     @@@     .@.
    //            ...     .@.     ..@     @@.
    //
    // green 
    // S:  @@     .@@     .@.     ...     @..
    //    @X      @@.     .@@     .@@     @@.
    //            ...     ..@     @@.     .@.
    //
    // red 
    // Z: @@      @@.     ..@     ...     .@.
    //     X@     .@@     .@@     @@.     @@.
    //            ...     .@.     .@@     @..
    //
    // purple 
    // T:  @      .@.     .@.     ...     .@.
    //    @X@     @@@     .@@     @@@     @@.
    //            ...     .@.     .@.     .@.
    //
    // yellow 
    // O: @@      .@@.    .@@.    .@@.    .@@.
    //    X@      .@@.    .@@.    .@@.    .@@.
    //            ....    ....    ....    ....
    //
    // cyan
    // I:         ....    ..@.    ....    .@..
    //    @X@@    @@@@    ..@.    ....    .@..
    //            ....    ..@.    @@@@    .@..
    //            ....    ..@.    ....    .@..
    public enum Shape
    {
        I, O, T, S, Z, J, L
    }
}