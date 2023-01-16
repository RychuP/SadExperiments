using SadExperiments.Pages;

namespace SadExperiments.MainScreen;

internal class Container : ScreenObject
{
    readonly Header _header;
    readonly Page[] _pages =
    {
        new WelcomePage(),
        new AnimatedGlobe(),
        new Donut3dPage(),
        new A_Demo(),
        new SplashScreens(),
        new SmoothScrolling(),
        new RectangleManipulation(),
        new StringParser(),
        new FontLoading(),
        new KeyboardAndMouse(),
        new BasicDrawing(),             // tutorial part 1
        new PlayingWithCursor(),        // tutorial part 2.1
        new OverlappingConsoles(),      // tutorial part 2.2
        new KeyboardProcessing(),
        new UpdateAndRender(),
        new BinaryOperationsPage(),
        new CharsAndCursors(),
        new MovableCharacter(),
        new MoveAndResize(),
        new FocusSetting(),
        new SurfaceShifting(),
        new CheckeredRoom(),
        new PixelNoise(),
        new CellSurfaceResizing(),
        new EasingFunctions(),
        new ImageConversion(),
        new SadCanvasPage(),
        new Instructions(),
        new EffectsAndDecorators(),
    };

    public Container()
    {
        var firstPage = _pages[0];
        firstPage.IsFocused = true;

        _header = new(firstPage, _pages.Length);

        Game.Instance.Screen = this;
        Game.Instance.DestroyDefaultStartingConsole();

        Children.Add(_header);
        Children.Add(firstPage);
    }

    public Page Page
    {
        get => Children[1] as Page ?? throw new Exception("Container has not got a Page added to its Children.");
        set
        {
            if (Children.Count >= 2)
                Children[1] = value;
            else if (Children.Count == 1)
                Children.Add(value);
            else
                throw new Exception("Trying to set a Page in Children at a different index than 1.");
        }
    }

    public void NextPage() => ChangePage(_pages.Length - 1, 1, _pages[0]);

    public void PrevPage() => ChangePage(0, -1, _pages.Last());

    void ChangePage(int testIndex, int step, Page overlappingPage)
    {
        // get the index of current page
        int currentPageIndex = Array.IndexOf(_pages, Page);
        Children.Remove(Page);

        // pull the next page from array and display it
        int nextIndex = currentPageIndex + step;
        var page = currentPageIndex == testIndex ? overlappingPage : _pages[nextIndex];
        page.IsFocused = true;
        Page = page;

        // change header title and summary to describe the page
        _header.SetHeader(Page, step == -1 ? Direction.Left : Direction.Right);
    }
}