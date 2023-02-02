using SadConsole.Quick;

namespace SadExperiments.Pages;

internal class SmoothScrolling : Page
{
    public SmoothScrolling() : base(Program.Width * 4, Program.Height * 4)
    {
        Title = "Smooth As Elvis";
        Summary = "Smoothest panning of random garbage on this planet.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Input, Tag.Keyboard };

        this.FillWithRandomGarbage(Font);
        this.WithKeyboard((host, k) =>
        {
            if (host is Console console && k.HasKeysDown)
            {
                if (k.IsKeyDown(Keys.Left))
                {
                    console.ViewPosition += Direction.Left;
                }

                else if (k.IsKeyDown(Keys.Right))
                {
                    console.ViewPosition += Direction.Right;
                }

                if (k.IsKeyDown(Keys.Up))
                {
                    console.ViewPosition += Direction.Up;
                }

                else if (k.IsKeyDown(Keys.Down))
                {
                    console.ViewPosition += Direction.Down;
                }
                return true;
            }
            return false;
        });
    }
}