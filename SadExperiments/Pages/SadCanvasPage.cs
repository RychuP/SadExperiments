using SadCanvas;
using Triangle = SadCanvas.Shapes.Triangle;

namespace SadExperiments.Pages;

internal class SadCanvasPage : Page
{
    public SadCanvasPage()
    {
        Title = "SadCanvas";
        Summary = "Testing the Canvas class from the SadCanvas nuget.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.SadCanvas, Tag.Pixels, Tag.Drawing };

        var canvas = new Canvas(200, 100, Color.Yellow)
        {
            Parent = this,
            UsePixelPositioning = false,
            Position = (5, 3)
        };

        var textSurface = GetTextSurface();
        canvas.Children.Add(textSurface);
        textSurface.Surface.Print(1, "Test");

        SetRandomPixels(canvas);

        canvas = new Canvas(10, 10, Color.LightBlue)
        {
            Parent = this,
            Position = (30, 30)
        };

        // texture rectangle areas
        var t = canvas.Texture;
        int amount = 25;
        var data = new MonoColor[amount];
        Array.Fill(data, MonoColor.LightSalmon);
        var r = new Rectangle(5, 5, 5, 5);
        t.SetData(0, r.ToMonoRectangle(), data, 0, data.Length);

        // bottom right canvas
        canvas = new Canvas(200, 100)
        {
            Parent = this
        };
        canvas.Fill(Color.Red);
        canvas.Position = (Width - canvas.CellWidth - 5, Height - canvas.CellHeight - 3); 

        SetRandomPixels(canvas);

        textSurface = GetTextSurface();
        canvas.Children.Add(textSurface);
        textSurface.Position = (canvas.CellWidth - textSurface.Surface.Width - 1, 
            canvas.CellHeight - textSurface.Surface.Height - 1);
        textSurface.Surface.Print(1, "Canvas");

        canvas = new ProceduralTriangles((WidthPixels / 3) * 2, (HeightPixels / 3) * 2)
        {
            Parent = this,
            UsePixelPositioning = true
        };
        canvas.Position = (WidthPixels / 2 - canvas.Width / 2, HeightPixels / 2 - canvas.Height / 2);
        Children.MoveToBottom(canvas);
    }

    void SetRandomPixels(Canvas canvas)
    {
        for (int i = 0; i < 400; i++)
        {
            int x = Game.Instance.Random.Next(0, canvas.Width);
            int y = Game.Instance.Random.Next(0, canvas.Height);
            Color c = Program.RandomColor;
            Point p = (x, y);
            canvas.SetPixel(p, c);
        }
    }

    static ScreenSurface GetTextSurface()
    {
        var ts = new ScreenSurface(10, 3) { Position = (1, 1) };
        ts.Surface.SetDefaultColors(Color.White, Color.Green);
        return ts;
    }

    class ProceduralTriangles : Canvas
    {
        public ProceduralTriangles(int width, int height) : base(width, height)
        {
            int colHeight = 15;
            int colWidth = 12;
            int w = Width / colWidth;
            int h = Height / colHeight;
            int w2 = w / 2;
            int h2 = h / 2;
            Point s = (0, 0);

            for (int x = 0; x <= colWidth; x++)
            {
                for (int y = 0; y < colHeight; y++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        s = (Side)i switch
                        {
                            Side.Top => s,
                            Side.Right => s + (w, 0),
                            Side.Bottom => s + (0, h),
                            _ => s - (w, 0)
                        };

                        Color c = Program.RandomColor;
                        Triangle t = (Side)i switch
                        {
                            Side.Top => new Triangle(s, s + (w, 0), s + (w2, h2), c, c),
                            Side.Right => new Triangle(s, s + (0, h), s + (-w2, h2), c, c),
                            Side.Bottom => new Triangle(s, s - (w, 0), s - (w2, h2), c, c),
                            _ => new Triangle(s, s - (0, h), s + (w2, -h2), c, c)
                        };

                        DrawPolygon(t, true);
                    }
                }
                s = (x * w, 0);
            }
        }

        enum Side { Top, Right, Bottom, Left }
    }
}