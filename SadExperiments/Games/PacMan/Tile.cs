namespace SadExperiments.Games.PacMan;

abstract class Tile : ColoredGlyph
{
    public Point Position { get; init; }
    public Tile(Point position, Color fg, int glyph) : base(fg, Color.Transparent, glyph)
    {
        Position = position;
    }
}

class Floor : Tile
{
    public Floor(Point position) : base(position, Appearances.Floor.Foreground, Appearances.Floor.Glyph)
    { }
}

class Wall : Tile
{
    // used for bit rotation
    static readonly int[] _bitsRotatedClockwise = { 6, 3, 0, 7, 4, 1, 8, 5, 2 };

    // 000
    // 010  the bit in the center is always set
    // 000  and represents this wall
    int _neighbours = 16;

    public Wall(Point position) : base(position, Appearances.Wall.Foreground, Appearances.Wall.Glyph)
    { }

    public void SetAppearance(IEnumerable<Wall> walls, bool isPerimeter)
    {
        SetNeighbours(walls);

        if (!isPerimeter)
            SetGlyph(Appearances.InnerWalls);
        else
            SetGlyph(Appearances.PerimeterWalls);
    }

    void SetNeighbours(IEnumerable<Wall> walls)
    {
        foreach (var wall in walls)
        {
            // get the delta difference between two points and add 1 (for the index getting purpose)
            Point p = wall.Position - Position + (1, 1);

            // convert the point to index where 0 is upright and 8 is bottomdown in relation to Position
            int i = p.ToIndex(3);

            // shift the 256 bit i times to the right to mark where the neighbour is
            int flag = 256 >> i;

            // store the neighbour position as the bit
            _neighbours |= flag;
        }
    }

    void SetGlyph(Dictionary<int, int> glyphs)
    {
        for (int i = 0; i < 4; i++)
        {
            if (glyphs.ContainsKey(_neighbours))
            {
                // small hack to cover the imposibility of distinguishing straight perimeter walls 
                // between left and right or top and bottom when no additional features are present
                int index = glyphs == Appearances.PerimeterWalls && _neighbours == 56
                    && (Position.X == 0 || Position.Y == 0) ? i + 2 : i;

                Glyph = glyphs[_neighbours] + index;
                return;
            }
            else
                RotateNeighboursClockwise();
        }
    }

    void RotateNeighboursClockwise()
    {
        int temp = 0;

        for (int i = 0; i < 9; i++)
        {
            // get the bit number from neighbours which is at the i index in indices array
            int flag = 256 >> _bitsRotatedClockwise[i];

            // copy the bit from neighbours
            int bit = _neighbours & flag;

            // shift the bit back to position 0;
            bit <<= _bitsRotatedClockwise[i];

            // shift the bit to its new position after rotation
            bit >>= i;

            // set the copied bit to temp
            temp ^= bit;
        }

        _neighbours = temp;
    }
}