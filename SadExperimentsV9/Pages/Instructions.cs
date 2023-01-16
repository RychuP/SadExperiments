using SadConsole.Instructions;

namespace SadExperiments.Pages;

internal class Instructions : Page
{
    public Instructions()
    {
        Title = "Instructions";
        Summary = "Shows the use of some SadConsole instructions.";

        int gradientPositionX = -50, gradientChange = 1, angle = 45; //angleChange = 15;
        var logoText = new Gradient(new[] { Color.Magenta, Color.Yellow }, new[] { 0.0f, 1f })
                           .ToColoredString("[| Powered by SadConsole |]");
        var logoText2 = new Gradient(Color.Magenta, Color.Yellow)
                            .ToColoredString("[| Powered by SadConsole |]");

        var c = new Console(Width, Height, GetRandomBackgroundGlyphs(Width * Height));
        Children.Add(c);
        c.Print(2, 2, AbsolutePosition.ToString(), Color.White, Color.Black);

        InstructionSet animation = new InstructionSet()

                // Animation to move the angled gradient spotlight effect
                .Code(MoveGradient)

                // Clear the background text so new printing doesn't look bad
                .Code((host, delta) =>
                {
                    ((IScreenSurface)host).Surface.Fill(Color.Black, Color.Transparent, 0);
                    return true;
                })

                // Draw the SadConsole text at the bottom
                .InstructConcurrent(
                    new DrawString(logoText)
                    {
                        Position = new Point(26, Height - 3),
                        TotalTimeToPrint = 4f
                    },

                    new DrawString(logoText2)
                    {
                        Position = new Point(26, Height - 1),
                        TotalTimeToPrint = 2f
                    }
                );

        animation.RemoveOnFinished = true;

        c.SadComponents.Add(animation);

        bool MoveGradient(IScreenObject console, TimeSpan delta)
        {
            gradientPositionX += gradientChange;

            if (gradientPositionX > Width + 50)
            {
                return true;
            }
            /*
            if ( (gradientChange == 1 && gradientPositionX > Width + 50) || (gradientChange == -1 && gradientPositionX < -50) )
            {
                gradientChange = -gradientChange;
                angle += angleChange;

                if (angle == 90 || angle == 45)
                {
                    angleChange = -angleChange;
                }
            }
            */

            Color[] colors = new[] { Color.Black, Color.Green, Color.White, Color.Blue, Color.Black };
            float[] colorStops = new[] { 0f, 0.2f, 0.5f, 0.8f, 1f };

            Algorithms.GradientFill(
                c.FontSize,
                new Point(gradientPositionX, 12),
                20,
                angle,
                new Rectangle(0, 0, Width, Height),
                new Gradient(colors, colorStops),
                (f, b, col) => ((IScreenSurface)c).Surface.SetForeground(f, b, col)
            );

            return false;
        }
    }

    // returns an array with a seed of characters for a background
    public static ColoredGlyph[] GetRandomBackgroundGlyphs(int count)
    {
        var cg = new List<ColoredGlyph>();
        for (int i = 0; i < count; i++)
        {
            cg.Add(new ColoredGlyph(Program.RandomColor, Program.RandomColor, 177));
        }
        return cg.ToArray();
    }
}