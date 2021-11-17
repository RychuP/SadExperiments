using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadConsole;
using SadConsole.Components;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace SadExperimentsV9.TestConsoles
{
    /// <summary>
    /// Shows how to display font characters by index in seperate consoles with a help of cursors
    /// and how to manipulate individual pixels of a font character.
    /// </summary>
    internal class CharsAndCursors : Console
    {
        const int Border = 1, Gap = 2;
        Dictionary<int, (int Y, Color Color, Direction Direction)> _charMods = new();

        Point _lastPoint = Point.None;
        IEnumerator<Point> _points;
        TimeSpan _time;

        public CharsAndCursors() : base(Chars.W + Indices.W + Border * 2 + Gap, Chars.H + Title.H + Border)
        {
            DefaultBackground = Color.DarkCyan;
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
                    indices.Cursor.Print($"{charIndex :X2}");
                    if (x < 15) indices.Cursor.Right(1);
                }
                chars.MoveToNextLine();
                indices.MoveToNextLine();
            }

            _points = GetPoint();
        }

        Direction RandomDirection() => Game.Instance.Random.Next(0, 2) switch
        {
            0 => Direction.Up,
            _ => Direction.Down
        };

        int RandomCharRow() => Game.Instance.Random.Next(0, 16);

        // manipulating individual pixels of a "space" character -> red dot moving around the perimeter
        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            _time += delta;
            if (_time > TimeSpan.FromSeconds(0.2d))
            {
                _time = TimeSpan.Zero;

                for (int i = 2; i < 10; i++)
                {
                    var r = Font.GetGlyphSourceRectangle(i);
                    //ResetCharColors(i, r);
                    ApplyEffect(i, r);
                }

                if (_lastPoint != Point.None)
                {
                    Font.Image.SetPixel(_lastPoint, Color.Transparent);
                }

                _points.MoveNext();
                _lastPoint = _points.Current;

                Font.Image.SetPixel(_lastPoint, Color.Red);
                IsDirty = true;
            }
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

            public Title(Console p) : base(p.Width, H)
            {
                Parent = p;
                Surface.Print(0, 1, $"Font: {Font.Name}".Align(HorizontalAlignment.Center, p.Width));
            }
        }

        class Indices : Test
        {
            public const int W = Chars.W * 3 - 1, H = Chars.H;

            public Indices(Console parent) : base(W, H)
            {
                Parent = parent;
                Position = (Border + Chars.W + Gap, Title.H);
            }
        }

        class Chars : Test
        {
            public const int W = 16, H = 16 + 1;

            public Chars(Console parent) : base(W, H)
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
}
