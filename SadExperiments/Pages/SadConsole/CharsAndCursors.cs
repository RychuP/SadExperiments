namespace SadExperiments.Pages;

class CharsAndCursors : Page
{
    public CharsAndCursors()
    {
        Title = "Chars And Cursors";
        Summary = "Font breakdown in separate consoles and playing with pixels.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.SadFont, Tag.Cursor, Tag.Pixels };
        
        AddCentered(new FontBreakdown());
        Surface.Print(2, ColoredString.Parser.Parse("If you look closely, there is a " +
            "[c:r f:ansiredbright]red dot[c:undo] circling the space character."));
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        (SubPage as FontBreakdown)!.RemoveRedDot();
        base.OnParentChanged(oldParent, newParent);
    }
}

/// <summary>
/// Shows how to display font characters in seperate consoles with a help of cursors
/// and how not to manipulate individual pixels of a font character using SetPixel/GetPixel methods (super slow).
/// </summary>
internal class FontBreakdown : SubPage
{
    const int _numberOfCharsForTheEffect = 10;

    const int Border = 1, Gap = 2;
    Dictionary<int, (int Y, Color Color, Direction Direction)> _charMods = new();

    Point _lastPoint = Point.None;
    IEnumerator<Point> _points;
    TimeSpan _time;

    public FontBreakdown() : base(Chars.W + Indices.W + Border * 2 + Gap, Chars.H + Title.H + Border)
    {
        Surface.DefaultBackground = Color.DarkCyan;
        Surface.Clear();

        var title = new Title(this);
        var chars = new Chars(this);
        var indices = new Indices(this);

        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                int charIndex = y * 16 + x;

                // generate random character effect
                _charMods[charIndex] = (RandomCharRow(), Program.RandomColor, RandomDirection());

                // left window char symbols
                string t = $"{(char)charIndex}";
                chars.Cursor.Print(t);

                // right window char indices as hex
                indices.Cursor.Print($"{charIndex:X2}");
                if (x < 15) indices.Cursor.Right(1);
            }
            chars.MoveToNextLine();
            indices.MoveToNextLine();
        }

        _points = GetPoint();
    }

    static Direction RandomDirection() => Game.Instance.Random.Next(0, 2) switch
    {
        0 => Direction.Up,
        _ => Direction.Down
    };

    static int RandomCharRow() => Game.Instance.Random.Next(0, 16);

    // manipulating individual pixels of a "space" character -> red dot moving around the perimeter
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        _time += delta;
        if (_time > TimeSpan.FromSeconds(0.2d))
        {
            _time = TimeSpan.Zero;

            for (int i = 2; i < _numberOfCharsForTheEffect; i++)
            {
                var r = Font.GetGlyphSourceRectangle(i);
                ApplyEffect(i, r);
            }

            // move the red dot around the space character
            RemoveRedDot();
            SetRedDot();

            IsDirty = true;
        }
    }

    // removes the red dot from the space character
    public void RemoveRedDot()
    {
        if (_lastPoint != Point.None)
        {
            Font.Image.SetPixel(_lastPoint, Color.Transparent);
        }
    }

    // sets the red color at the next point in the circumference of the space character
    void SetRedDot()
    {
        _points.MoveNext();
        _lastPoint = _points.Current;
        Font.Image.SetPixel(_lastPoint, Color.Red);
    }

    void ApplyEffect(int charIndex, Rectangle r)
    {
        var mod = _charMods[charIndex];

        if (mod.Direction == Direction.Down)
        {
            if (++mod.Y > 15)
                UpdateTuple(14, Direction.Up);
            else
                _charMods[charIndex] = mod;
        }
        else
        {
            if (--mod.Y < 0)
                UpdateTuple(1, Direction.Down);
            else
                _charMods[charIndex] = mod;
        }

        var c1 = Program.RandomColor;

        for (int x = 0; x < 8; x++)
        {
            Point p = (r.X + x, r.Y + mod.Y);
            Color c = Font.Image.GetPixel(p);
            if (c != Color.Transparent)
                Font.Image.SetPixel(p, c1);
        }

        void UpdateTuple(int y, Direction d)
        {
            mod.Y = y;
            mod.Direction = d;
            _charMods[charIndex] = mod;
        }
    }

    void ResetCharColors(int charIndex, Rectangle r)
    {
        for (int y = 0; y < r.Height; y++)
        {
            for (int x = 0; x < r.Width; x++)
            {
                Point p = (r.X + x, r.Y + y);
                Color currentPixelColor = Font.Image.GetPixel(p);
                if (currentPixelColor != Color.Transparent && currentPixelColor != Color.White)
                    Font.Image.SetPixel(p, Color.White);
            }
        }
    }

    IEnumerator<Point> GetPoint()
    {
        for (int side = 0; side <= 4; side++)
        {
            if (side == 4) side = 0;

            for (int i = 0, sideLength = side % 2 == 0 ? 8 : 16; i < sideLength; i++)
            {
                Point p = side switch
                {
                    0 => (1 + i, 1),
                    1 => (8, 1 + i),
                    2 => (8 - i, 16),
                    _ => (1, 16 - i)
                };

                yield return p;
            }
        }
    }

    class Title : ScreenSurface
    {
        public const int H = 3;

        public Title(ScreenSurface p) : base(p.Surface.Width, H)
        {
            Parent = p;
            Surface.Print(1, $"Font: {Font.Name}");
        }
    }

    class Indices : Test
    {
        public const int W = Chars.W * 3 - 1, H = Chars.H;

        public Indices(ScreenSurface parent) : base(W, H)
        {
            Parent = parent;
            Position = (Border + Chars.W + Gap, Title.H);
        }
    }

    class Chars : Test
    {
        public const int W = 16, H = 16 + 1;

        public Chars(ScreenSurface parent) : base(W, H)
        {
            Parent = parent;
            Position = (Border, Title.H);
        }

        public void Test()
        {
            Font.Image.SetPixel((2, 2), Color.Red);
            IsDirty = true;
        }
    }

    class Test : Console
    {
        public Test(int w, int h) : base(w, h)
        {
            DefaultBackground = Color.DarkGray;
            Surface.Clear();
        }

        public void MoveToNextLine()
        {
            Cursor.CarriageReturn();
        }
    }
}