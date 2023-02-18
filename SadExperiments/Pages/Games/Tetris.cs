using GoRogue.Random;
using SadConsole.Components;
using SadConsole.Entities;
using SadConsole.UI;

namespace SadExperiments.Pages;

class Tetris : Page
{
    public static readonly int SolidGlyph = Fonts.C64.SolidGlyphIndex;

    public TetrisBoard Board;

    public Tetris()
    {
        #region Meta
        Title = "Tetris";
        Summary = "Mini game (work in progress).";
        Submitter = Submitter.Rychu;
        Date = new(2023, 01, 01);
        Tags = Array.Empty<Tag>();
        #endregion Meta

        // create tetris board elements
        Board = new TetrisBoard() { Position = (2, 0) };
        var parameters = Border.BorderParameters.GetDefault().ChangeBorderForegroundColor(Color.Pink);
        var border = new Border(Board, parameters) { Position = (1, -1) };

        // a mask covering the top two board rows (tetromino spawn area) shifted up by a few pixels 
        // as per guidance https://tetris.fandom.com/wiki/Tetris_Guideline
        var mask = new ScreenSurface(border.Surface.Width, 2)
        {
            UsePixelPositioning = true,
            Position = border.Position.Translate(0, 1) * border.FontSize - (0, 5),
            Font = border.Font,
            FontSize = border.FontSize,
        };
        for (int i = 0; i < border.Surface.Width; i++)
            mask.Surface.SetCellAppearance(i, 1, border.Surface[i]);

        // add all board elements to children
        Children.Add(border, Board, mask);

        // info 
        Surface.Print(35, 2, "Controls: z, x, left, right, down");
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        // reduce initial keyboard repeat delay to make the moves left and right start faster
        Game.Instance.Keyboard.InitialRepeatDelay = newParent is Container ? 0.3f : 
            Container.Instance.DefaultInitialRepeatDelay;
        base.OnParentChanged(oldParent, newParent);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        // slower moves
        if (keyboard.HasKeysPressed)
        {
            if (keyboard.IsKeyPressed(Keys.X))
                Board.RotateTetrominoRight();
            else if (keyboard.IsKeyPressed(Keys.Z))
                Board.RotateTetrominoLeft();
            else if (keyboard.IsKeyPressed(Keys.Left))
                Board.MoveTetrominoLeft();
            else if (keyboard.IsKeyPressed(Keys.Right))
                Board.MoveTetrominoRight();
        }

        // faster moves
        if (keyboard.HasKeysDown)
        {
            if (keyboard.IsKeyDown(Keys.Down))
                Board.MoveTetrominoDown();
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
    Tetromino _current = Tetromino.Next();
    int _gravity = 1;
    const int MaxGravity = 20;
    Timer _timer;
    #endregion Fields

    #region Constructors
    public TetrisBoard() : base(10, 22)
    {
        Font = Fonts.Square10;
        FontSize *= 2;

        Current = _current;
        SadComponents.Add(_renderer);

        _timer = new(TimeSpan.FromSeconds(0.6d));
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
            _renderer.Add(value);
            _current = value;
        }
    }

    public Tetromino Next { get; private set; } = Tetromino.Next();

    public int Score { get; private set; }
    #endregion Properties

    #region Methods
    public void RotateTetrominoLeft() => RotateTetromino(Current.RotateLeft, Current.RotateRight);
    public void RotateTetrominoRight() => RotateTetromino(Current.RotateRight, Current.RotateLeft);
    public void MoveTetrominoLeft() => MoveTetrominoHorizontally(Current.MoveLeft, Current.MoveRight);
    public void MoveTetrominoRight() => MoveTetrominoHorizontally(Current.MoveRight, Current.MoveLeft);
    public void MoveTetrominoDown()
    {
        Current.MoveDown();

        if (!LocationIsValid())
        {
            Current.MoveUp();
            PlantCurrentTetromino();
            return;
        }
    }

    void Timer_OnTimerElapsed(object? o, EventArgs e)
    {
        MoveTetrominoDown();
    }

    void PlantCurrentTetromino()
    {
        Current = Next;
        Next = Tetromino.Next();
        RemoveFullRows();
        OnTetrominoPlanted();
    }

    void RemoveFullRows()
    {
        // start at the bottom row
        int row = Surface.Height - 1;

        for (int i = 0; i < 20; i++)
        {
            // get all blocks with the row number
            var blocks = _renderer.Entities.Where(b => b.Position.Y == row).ToArray();

            // check if the row is full
            if (blocks.Count() == Surface.Width)
            {
                // remove all blocks in the row
                foreach (var block in blocks)
                    _renderer.Remove(block);

                // move all higher rows down
                foreach (var block in _renderer.Entities)
                    if (block.Position.Y < row)
                        block.Position = block.Position.Translate(0, 1);

                // skip changing the row number
                continue;
            }

            // move to the higher row
            row--;
        }
    }

    void MoveTetrominoHorizontally(Action desiredMove, Action reversedMove)
    {
        desiredMove();
        if (!LocationIsValid())
            reversedMove();
    }

    void RotateTetromino(Action desiredMove, Action reversedMove)
    {
        desiredMove();
        if (!LocationIsValid() && !KickLocationIsValid())
            reversedMove();
    }

    bool KickLocationIsValid()
    {
        Current.MoveLeft();
        if (LocationIsValid()) return true;

        Current.MoveRight();
        Current.MoveRight();
        if (LocationIsValid()) return true;

        Current.MoveLeft();
        Current.MoveDown();
        if (LocationIsValid()) return true;

        Current.MoveLeft();
        if (LocationIsValid()) return true;

        Current.MoveRight();
        Current.MoveRight();
        if (LocationIsValid()) return true;

        Current.MoveLeft();
        Current.MoveUp();
        return false;
    }

    bool LocationIsValid()
    {
        if (!CheckBlocksInBounds() || !CheckCollisionsWithOthers())
            return false;
        return true;
    }

    bool CheckCollisionsWithOthers()
    {
        _renderer.Remove(Current);
        foreach (var block in Current.Blocks)
            if (_renderer.Entities.Any(b => b.Position == block.Position))
            {
                _renderer.Add(Current);
                return false;
            }
        _renderer.Add(Current);
        return true;
    }

    bool CheckBlocksInBounds()
    {
        foreach (var block in Current.Blocks)
            if (!Surface.Area.Contains(block.Position))
                return false;
        return true;
    }

    void OnTetrominoPlanted()
    {
        TetrominoPlanted?.Invoke(this, EventArgs.Empty);
    }
    #endregion Methods

    #region Events
    public event EventHandler? TetrominoPlanted;
    #endregion Events
}

class TetrominoManager : Renderer
{
    public void Add(Tetromino t)
    {
        foreach (Entity block in t.Blocks)
            Add(block);
    }

    public void Remove(Tetromino t)
    {
        foreach (Entity block in t.Blocks)
        {
            if (Entities.Contains(block))
                Remove(block);
        }
    }
}

class Tetromino
{
    // number of available tetromino shapes
    public static readonly int ShapeCount = Enum.GetNames(typeof(Shape)).Length;

    // bag of tetrominos to randomly select one from as per official guidance
    readonly static List<Tetromino> s_bag = new(ShapeCount);

    // shape of the tetromino
    public Shape Type { get; init; }

    // individual squares that form a tetromino
    public Entity[] Blocks { get; } = new Entity[4];

    public ColoredGlyph Appearance { get; init; }

    readonly int[,] _rotations;
    int _currentRotationIndex = 0;
    Rectangle _area;

    static void FillBag()
    {
        if (s_bag.Count > 0) s_bag.Clear();
        for (int i = 0; i < ShapeCount; i++)
            s_bag.Add(new Tetromino((Shape)i));
    }

    public static Tetromino Next()
    {
        if (s_bag.Count == 0) FillBag();
        int i = GlobalRandom.DefaultRNG.NextInt(s_bag.Count);
        Tetromino t = s_bag[i];
        s_bag.Remove(t);
        return t;
    }

    private Tetromino(Shape shape)
    {
        // SRS(Standard Rotation System) https://tetris.fandom.com/wiki/SRS
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

        int x = 4, y = 0;
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

    public enum Shape
    {
        I, O, T, S, Z, J, L
    }
}