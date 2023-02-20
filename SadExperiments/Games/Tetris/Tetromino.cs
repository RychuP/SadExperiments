using GoRogue.Random;
using SadConsole.Entities;

namespace SadExperiments.Games.Tetris;

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
    public Rectangle Area { get; private set; }

    static void FillBag()
    {
        if (s_bag.Count > 0) s_bag.Clear();
        for (int i = 0; i < ShapeCount; i++)
            s_bag.Add(new Tetromino((Shape)i));
    }

    public static void ResetBag()
    {
        s_bag.Clear();
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
        Area = shape switch
        {
            Shape.I => new(x, y, 4, 4),
            Shape.O => new(x + 1, y, 2, 2),
            _ => new(x, y, 3, 3)
        };

        Type = shape;
        Appearance = new ColoredGlyph(fgColor, Color.Transparent, Game.SolidGlyph);

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
            Point pivotLocalPos = Point.FromIndex(pivotPosIndex, Area.Width);
            Point pivotGridPos = Blocks[0].Position;
            Area = Area.WithPosition(pivotGridPos - pivotLocalPos);

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
            var deltaPosition = Point.FromIndex(blockPositionIndex, Area.Width);
            Blocks[i].Position = Area.Position + deltaPosition;
        }
    }

    public enum Shape
    {
        I, O, T, S, Z, J, L
    }
}