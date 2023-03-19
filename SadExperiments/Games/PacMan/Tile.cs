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

class Portal : Floor
{
    public char Id { get; init; }

    public Portal(Point position, char id) : base(position)
    {
        Id = id;
    }
}

class Spawner : Wall
{
    public Spawner(Point position) : base(position)
    {
        Glyphs = Appearances.SpawnerWalls;
    }

    protected override int GetGlyph(IEnumerable<Wall> walls, int i)
    {
        return Glyphs[Neighbours];
    }
}

class SpawnerEntrance : Spawner
{
    public SpawnerEntrance(Point position) : base(position)
    {
        Glyphs = Appearances.SpawnerEntranceWalls;
        Foreground = Color.OrangeRed;
    }
}

class PerimeterWall : Wall
{
    public PerimeterWall(Point position) : base(position)
    {
        Glyphs = Appearances.PerimeterWalls;
    }

    protected override int GetGlyph(IEnumerable<Wall> walls, int i)
    {
        // straight perimeter line
        if (Neighbours == 56)
        {
            if ((i == 0 && Position.Y == 0) ||
                (i == 1 && Position.X == 0))
                return 14;
        }

        // square corner of an exit coridor
        else if (Neighbours == 432)
        {
            if ((i == 0 && walls.Any(w => w.Position == Position + Direction.Up && w.Glyph == 21)) ||
                (i == 1 && Position.Y == 0) ||
                (i == 2 && Position.X == 0) ||
                (i == 3 && Position.X != 0))
                return 28;
        }

        return Glyphs[Neighbours];
    }
}

class Wall : Tile
{
    // used for bit rotation
    static readonly int[] _bitsRotatedClockwise = { 6, 3, 0, 7, 4, 1, 8, 5, 2 };

    // 000
    // 010  the bit in the center is always set
    // 000  and represents this wall
    protected int Neighbours { get; private set; } = 16;

    protected Dictionary<int, int> Glyphs { get; set; } = Appearances.InnerWalls;

    public Wall(Point position) : base(position, Appearances.Wall.Foreground, Appearances.Wall.Glyph)
    { }

    public void SetAppearance(IEnumerable<Wall> walls)
    {
        SetNeighbours(walls);
        SetGlyph(walls);
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
            Neighbours |= flag;
        }
    }

    protected virtual int GetGlyph(IEnumerable<Wall> walls, int i)
    {
        // straight inner line (for single line walls; mainly square wall shapes)
        if (Neighbours == 146)
        {
            if (i == 0 && walls.Any(w => w.Position == Position + Direction.Up && w.Glyph == 6) ||
                i == 1 && walls.Any(w => w.Position == Position + Direction.Left && w.Glyph == 7))
                    return 6;
        }

        return Glyphs[Neighbours];
    }

    void SetGlyph(IEnumerable<Wall> walls)
    {
        for (int i = 0; i < 4; i++)
        {
            if (Glyphs.ContainsKey(Neighbours))
            {
                // pull the glyph index from the appearances array
                int glyphIndex = GetGlyph(walls, i);

                // rotate glyph index if it overflows the row
                int delta = glyphIndex % 4;
                Glyph = delta + i >= 4 ?
                    glyphIndex - 4 + i : glyphIndex + i;

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
            int bit = Neighbours & flag;

            // shift the bit back to position 0;
            bit <<= _bitsRotatedClockwise[i];

            // shift the bit to its new position after rotation
            bit >>= i;

            // set the copied bit to temp
            temp ^= bit;
        }

        Neighbours = temp;
    }
}