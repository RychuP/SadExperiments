using System;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Host;
using SadConsole;
using SadRogue.Primitives;
using MonoColor = Microsoft.Xna.Framework.Color;

namespace SadExperimentsV9.TestConsoles
{
    // My first attempt at creating a canvas (hacking way).
    // I've created a separate project for a dedicated class that does it properly.
    // Check out SadCanvas project.
    internal class FontTextureCanvas : ScreenSurface, IDisposable
    {
        private readonly Texture2D _texture;

        public MonoColor[] Cache { get; init; }

        public FontTextureCanvas(int width, int height, MonoColor? color = null) : base(1, 1)
        {
            _texture = new Texture2D(Global.GraphicsDevice, width, height);
            var gameTexture = new GameTexture(_texture);
            Font = new SadFont(width, height, 0, 1, 1, 1, gameTexture, "Canvas");
            Cache = new MonoColor[width * height];
            if (color != null)
            {
                Fill(color.Value);
                Draw();
            }
        }

        public void Fill(MonoColor color)
        {
            Array.Fill(Cache, color);
        }

        public void Draw()
        {
            _texture.SetData(Cache);
            IsDirty = true;
        }

        ~FontTextureCanvas()
        {
            _texture.Dispose();
            base.Dispose(false);
        }
    }
}
