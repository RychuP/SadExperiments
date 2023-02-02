using SadConsole.Instructions;
using SadExperiments;

namespace SadExperiments.Pages.Sad_Console;

internal class Instructions : Page, IRestartable
{
    int _gradientPositionX, _gradientChange = 1, _angle = 45;
    readonly ColoredString _logoText = new Gradient(new[] { Color.Magenta, Color.Yellow }, new[] { 0.0f, 1f })
                       .ToColoredString("[| Powered by SadConsole |]");
    readonly ColoredString _logoText2 = new Gradient(Color.Magenta, Color.Yellow)
                        .ToColoredString("[| Powered by SadConsole |]");
    public Instructions()
    {
        Title = "Instructions";
        Summary = "Shows the use of some SadConsole instructions.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Animations, Tag.Instructions, Tag.IComponent, Tag.GradientFill };
    }

    public void Restart()
    {
        _gradientPositionX = -50;

        Surface.Clear();
        FillWithRandomBackgroundGlyphs();

        // remove previous instructions if any
        var animationInstructions = SadComponents.Where(sc => sc is InstructionSet).FirstOrDefault();
        if (animationInstructions != null) SadComponents.Remove(animationInstructions);

        animationInstructions = new InstructionSet() { RemoveOnFinished = true }
            // move the angled gradient spotlight effect
            .Code(MoveGradient)

            // clear the background text so new printing doesn't look bad
            .Code((host, delta) =>
            {
                ((IScreenSurface)host).Surface.Fill(Color.Black, Color.Transparent, 0);
                return true;
            })

            // draw the SadConsole text at the bottom
            .InstructConcurrent(
                new DrawString(_logoText)
                {
                    Position = new Point(26, Height - 3),
                    TotalTimeToPrint = 4f
                },

                new DrawString(_logoText2)
                {
                    Position = new Point(26, Height - 1),
                    TotalTimeToPrint = 2f
                }
            );

        SadComponents.Add(animationInstructions);
    }

    void FillWithRandomBackgroundGlyphs()
    {
        for (int i = 0, count = Surface.Count; i < count; i++)
        {
            (int x, int y) = Point.FromIndex(i, Surface.Width);
            Surface.SetGlyph(x, y, new ColoredGlyph(Program.RandomColor, Program.RandomColor, 177));
        }
    }

    bool MoveGradient(IScreenObject console, TimeSpan delta)
    {
        _gradientPositionX += _gradientChange;

        if (_gradientPositionX > Width + 50)
        {
            return true;
        }

        Color[] colors = new[] { Color.Black, Color.Green, Color.White, Color.Blue, Color.Black };
        float[] colorStops = new[] { 0f, 0.2f, 0.5f, 0.8f, 1f };

        Algorithms.GradientFill(
            FontSize,
            new Point(_gradientPositionX, 12),
            20,
            _angle,
            new Rectangle(0, 0, Width, Height),
            new Gradient(colors, colorStops),
            (f, b, col) => Surface.SetForeground(f, b, col)
        );

        return false;
    }
}