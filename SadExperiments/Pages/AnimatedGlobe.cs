using SadConsole.Instructions;
using SadConsole.Readers;

namespace SadExperiments.Pages;

// Turning globe animation that uses instructions, image conversion and AnimatedScreenSurface class.
// Submitted to FeatureDemo project in Thraka's SadConsole repo.
class AnimatedGlobe : Page
{
    readonly TheDrawFont _drawFont;
    readonly ScreenSurface _animationScreen;
    readonly AnimatedGlobeClip _clip;
    readonly ScreenSurface _titleScreen;

    public AnimatedGlobe()
    {
        Title = "Animated Globe";
        Summary = "Uses instructions, image conversion and AnimatedScreenSurface class.";

        // draw font
        string fontFileName = "DESTRUCX.TDF";
        var fontEnumerable = TheDrawFont.ReadFonts($"Resources/Fonts/{fontFileName}");
        if (fontEnumerable is null) throw new ArgumentException();
        _drawFont = fontEnumerable.ToArray()[0];

        // animation screen
        _animationScreen = new ScreenSurface(Width, Height);

        // random noise simulating stars
        var staticNoise = AnimatedScreenSurface.CreateStatic(Width, Height, 48, 0.9d);
        staticNoise.AnimationDuration = 48 * 0.1f;
        _animationScreen.Children.Add(staticNoise);

        // globe animation
        _clip = new AnimatedGlobeClip(_animationScreen);

        // title screen
        _titleScreen = new ScreenSurface(Width, Height);
        _titleScreen.Surface.DefaultBackground = Color.Black;

        Start();
    }

    void Start()
    {
        _clip.Stop();
        Children.Add(_animationScreen);
        Children.Add(_titleScreen);

        // reset title screen
        byte opacity = 255;
        _titleScreen.Surface.Clear();  /* to make the whole surface opaque */
        _titleScreen.Surface.PrintTheDraw(3, "globe", _drawFont, HorizontalAlignment.Center);
        ((SadConsole.Renderers.ScreenSurfaceRenderer)_titleScreen.Renderer).Opacity = opacity;
        _titleScreen.Tint = Color.Black;

        // instructions
        var animationInstructions = new InstructionSet() { RemoveOnFinished = true }
            .Instruct(new FadeTextSurfaceTint(_titleScreen,
                new Gradient(Color.Black, Color.Transparent), TimeSpan.FromSeconds(1)))
            .Code(() =>
            {
                Children.Add(new SadConsole.SplashScreens.Simple());
            })
            .Wait(TimeSpan.FromSeconds(4))
            .Instruct(new FadeTextSurfaceTint(_titleScreen,
                new Gradient(Color.Transparent, Color.Black), TimeSpan.FromSeconds(1)))
            .Code(() =>
            {
                _clip.Start();
                _titleScreen.Surface.Clear();
                _titleScreen.Tint = Color.Transparent;
            })
            .Code((s, d) =>
            {
                ((SadConsole.Renderers.ScreenSurfaceRenderer)_titleScreen.Renderer).Opacity = opacity--;
                return opacity == 0 ? true : false;
            });
        SadComponents.Add(animationInstructions);
    }

    class AnimatedGlobeClip : AnimatedScreenSurface
    {
        static readonly (int Width, int Height, int Count, float Duration) _frame = (46, 46, 48, 0.17f);
        readonly Rectangle _frameArea;

        public AnimatedGlobeClip(ScreenSurface c) : base("Animated Globe", _frame.Width, _frame.Height / 2)
        {
            Parent = c;
            int x = (c.Surface.Width - Surface.Width) / 2,
                y = (c.Surface.Height - Surface.Height) / 2;
            Position = (x, y);

            // used to advance the point on the film where frames are copied from 
            _frameArea = new(0, 0, _frame.Width, _frame.Height);

            // convert png
            using ITexture sadImage = GameHost.Instance.GetTexture("Resources/Images/globe.png");
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