using Microsoft.Xna.Framework.Graphics;
using SadConsole.Host;

namespace SadExperiments.Pages;

class PixelNoise : Page
{
    public PixelNoise() 
    {
        Title = "Pixel Noise";
        Summary = "Font texture pixels set to random colors.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Pixels, Tag.Drawing, Tag.ScreenSurface };

        AddCentered(new FontTextureCanvas());
    }
}

// My first attempt at creating a canvas.
// I've created a separate project for a dedicated class that does it properly.
// Check out SadCanvas project.
internal class FontTextureCanvas : ScreenSurface, IDisposable
{
    private readonly Texture2D _texture;
    int _timer = 0;

    public MonoColor[] Cache { get; init; }

    public FontTextureCanvas(int width = 500, int height = 300) : base(1, 1)
    {
        _texture = new Texture2D(Global.GraphicsDevice, width, height);
        var gameTexture = new GameTexture(_texture);
        Font = new SadFont(width, height, 0, 1, 1, 1, gameTexture, "Canvas");
        Cache = new MonoColor[width * height];
    }

    public override void Update(TimeSpan delta)
    {
        if (_timer++ > 5)
        {
            _timer = 0;

            for (var i = 0; i < Cache.Length; i++)
            {
                Cache[i] = Program.RandomColor.ToMonoColor();
            }
        }

        base.Update(delta);
    }

    public override void Render(TimeSpan delta)
    {
        _texture.SetData(Cache);
        IsDirty = true;

        base.Render(delta);
    }

    ~FontTextureCanvas()
    {
        _texture.Dispose();
        base.Dispose(false);
    }
}
