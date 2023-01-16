using SadConsole.Quick;

namespace SadExperiments.Pages;

internal class SplashScreens : Page
{
    public SplashScreens()
    {
        Title = "Splash Screens";
        Summary = "Press either space or shift to see both splash screens.";

        this.WithKeyboard((host, keyboard) =>
        {
            if (host is Console c && keyboard.HasKeysPressed)
            {
                c.DefaultBackground = Program.RandomColor;
                c.Clear();
                c.Children.Clear();
                if (keyboard.IsKeyPressed(Keys.Space))
                {
                    c.Children.Add(new SadConsole.SplashScreens.Simple());

                }
                else if (keyboard.IsKeyPressed(Keys.LeftShift))
                {
                    c.Children.Add(new SadConsole.SplashScreens.PCBoot());
                }
                return false;
            }
            else
            {
                return false;
            }
        });
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Container)
        {
            Children.Clear();
            Children.Add(new SadConsole.SplashScreens.PCBoot());
        }
        base.OnParentChanged(oldParent, newParent);
    }
}