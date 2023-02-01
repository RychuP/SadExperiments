using SadConsole.SplashScreens;

namespace SadExperiments.Pages;

internal class SplashScreens : Page, IRestartable
{
    readonly SplashScreensList _splashScreens = new();

    public SplashScreens()
    {
        Title = "Splash Screens";
        Summary = "Press space to toggle between available splash screens.";
        Restart();
    }

    public void Restart()
    {
        ClearPage();
        Children.Add(_splashScreens.First());
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed && keyboard.IsKeyPressed(Keys.Space))
        {
            ClearPage();
            Children.Add(_splashScreens.Next());
            return true;
        }

        return base.ProcessKeyboard(keyboard);
    }

    void ClearPage()
    {
        DefaultBackground = Program.RandomColor;
        Surface.Clear();
        Children.Clear();
    }

    class SplashScreensList
    {
        int _currentIndex = 0;

        public ScreenSurface Next()
        {
            _currentIndex++;
            if (_currentIndex >= Count)
                _currentIndex = 0;
            return Current();
        }

        public const int Count = 2;
        public ScreenSurface Current()
        {
            return _currentIndex switch
            {
                0 => new Simple(),
                _ => new PCBoot()
            };
        }

        public ScreenSurface First()
        {
            _currentIndex = 0;
            return Current();
        }
    }
}