using System.Linq;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;

namespace SadExperimentsV9
{
    /*
     * Turning globe animation that uses image conversion and AnimatedScreenSurface class.
     * 
     * Submitted to FeatureDemo project in Thraka's SadConsole repo.
     * 
     */

    class AnimatedGlobe : Console
    {
        public AnimatedGlobe(int w, int h) : base(w, h)
        {
            // random noise simulating stars
            var staticNoise = AnimatedScreenSurface.CreateStatic(w, h, 48, 0.9d);
            staticNoise.AnimationDuration = 48 * 0.1f;
            Children.Add(staticNoise);

            // the animated globe
            var clip = new AnimatedGlobeClip(this);
            clip.Start();
        }
    }

    class AnimatedGlobeClip : AnimatedScreenSurface
    {
        static readonly (int Width, int Height, int Count, float Duration) _frame = (46, 46, 48, 0.17f);
        readonly Rectangle _frameArea;

        public AnimatedGlobeClip(SadConsole.Console c) : base("Animated Globe", _frame.Width, _frame.Height / 2)
        {
            Parent = c;
            int x = (c.Width - Surface.Width) / 2,
                y = (c.Height - Surface.Height) / 2;
            Position = (x, y);

            // used to advance the point on the film where frames are copied from 
            _frameArea = new(0, 0, _frame.Width, _frame.Height);

            // convert png
            using ITexture sadImage = GameHost.Instance.GetTexture("Images/globe.png");
            var fontSize = Game.Instance.DefaultFont.GetFontSize(Game.Instance.DefaultFontSize);
            var fontSizeRatio = Game.Instance.DefaultFont.GetGlyphRatio(fontSize);
            var frames = sadImage.ToSurface(TextureConvertMode.Foreground, _frame.Width * _frame.Count, _frame.Height / 2);

            // create frames from the raw data
            for (int i = 0; i < _frame.Count; i++)
            {
                var frame = CreateFrame();
                frames.Copy(_frameArea, frame, 0, 0);

                // fill the black cells with full alpha
                foreach (var cell in Frames[i])
                {
                    if (cell.Foreground.GetBrightness() < 1) cell.Background = cell.Background.FillAlpha();
                }

                // move the view to the next frame
                _frameArea = _frameArea.ChangeX(_frame.Width);
            }

            AnimationDuration = _frame.Count * _frame.Duration;
            Repeat = true;
        }
    }
}
