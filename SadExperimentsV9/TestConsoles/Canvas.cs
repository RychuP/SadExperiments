using System;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Host;
using SadConsole;
using SadRogue.Primitives;
using MonoColor = Microsoft.Xna.Framework.Color;

namespace SadExperimentsV9.TestConsoles
{
    internal class Canvas : ScreenSurface, IDisposable
    {
        private readonly Texture2D _texture;

        public MonoColor[] Cache { get; init; }

        public Canvas(int width, int height, MonoColor? color) : base(1, 1)
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
        }

        ~Canvas()
        {
            _texture.Dispose();
            base.Dispose(false);
        }
    }
}
