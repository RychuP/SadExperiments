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

    public void SetAppearance(IEnumerable<Wall> walls, PerimeterWall pw)
    {
        SetNeighbours(walls);
        SetGlyph(pw);
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

    void SetGlyph(PerimeterWall pw)
    {
        // pull appropriate glyphs according to the wall type
        var glyphs = pw == PerimeterWall.None ? Appearances.InnerWalls : Appearances.PerimeterWalls;

        for (int i = 0; i < 4; i++)
        {
            if (glyphs.ContainsKey(_neighbours))
            {
                // pull the glyph index from the appearances array
                int glyphIndex = glyphs[_neighbours];

                // some perimeter walls need additional checks
                if (pw != PerimeterWall.None)
                {
                    if (_neighbours == 56 && (pw == PerimeterWall.Left || pw == PerimeterWall.Top))
                        glyphIndex += 2;
                    else if (_neighbours == 432)
                    {
                        if ((i == 0 && pw == PerimeterWall.Right) ||
                            (i == 1 && pw == PerimeterWall.Top) ||
                            (i == 2 && pw == PerimeterWall.Left) || 
                            (i == 3 && pw == PerimeterWall.Bottom) )
                            glyphIndex = 28;
                    }
                }

                // if the glyph is not in the column 0, the overflow (glyphs index 4 and above) has to be rolled over
                int offset = i;
                int delta = glyphIndex % 4;
                if (delta + i >= 4)
                    offset = delta - offset + 1;

                // set the glyph and return
                Glyph = glyphIndex + offset;
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