using SadConsole.UI.Controls;
using SadConsole.UI;
using SadExperiments.Pages;

namespace SadExperiments.MainScreen;

internal class Container : ScreenObject
{
    readonly Header _header;
    readonly ContentsList _contentsList;
    Page _currentPage;
    readonly Page[] _pages =
    {
        new WelcomePage(),
        new AnimatedGlobe(),
        new Donut3dPage(),
        new A_Demo(),
        new Primitives(),
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
        // set first page as focused
        var firstPage = _pages[0];
        firstPage.IsFocused = true;

        // create a header
        _header = new(firstPage, _pages.Length);

        // instantiate data fields
        _contentsList = new ContentsList(ButtonsWithLinksToPages);
        _currentPage = firstPage;

        // remove starting console
        Game.Instance.Screen = this;
        Game.Instance.DestroyDefaultStartingConsole();

        // add children
        Children.Add(_header, _contentsList, _currentPage);

        // set page indices
        int i = 0;
        Array.ForEach(_pages, p => p.Index = i++);
    }

    public void NextPage() => ChangePage(Direction.Right);

    public void PrevPage() => ChangePage(Direction.Left);

    void ChangePage(Direction direction)
    {
        if (_contentsList.IsBeingShown) HideContentsList();
        int nextIndex = _currentPage.Index + (direction == Direction.Right ? 1 : -1);
        var page = nextIndex < 0              ? _pages.Last() :
                   nextIndex >= _pages.Length ? _pages.First() :
                                                _pages[nextIndex];
        ChangePage(page);
    }

    void ChangePage(Page page)
    {
        Children.Remove(_currentPage);
        Children.Add(page);
        _currentPage = page;
        _currentPage.IsFocused = true;
        _header.SetHeader(page);
        if (page is IRestartable p)
            p.Restart();
    }

    public void ShowContentsList()
    {
        if (!_contentsList.IsBeingShown)
        {
            Children.MoveToTop(_contentsList);
            _contentsList.IsVisible = true;
            _contentsList.IsFocused = true;
        }
        else HideContentsList();
    }

    void HideContentsList()
    {
        Children.MoveToTop(_currentPage);
        _contentsList.IsVisible = false;
        _currentPage.IsFocused = true;
        if (_currentPage is IRestartable p)
            p.Restart();
    }

    ControlsConsole ButtonsWithLinksToPages
    {
        get
        {
            var contentsList = new ControlsConsole(Program.Width, Program.Height);
            Point position = (1, 1);
            int buttonWidth = 35;

            foreach (Page page in _pages)
            {
                // create new button with a link to the page
                var button = new Button(buttonWidth, 1)
                {
                    Text = page.Title,
                    Position = position,
                    UseMouse = true,
                    UseKeyboard = false,
                };
                button.Click += (o, e) =>
                {
                    HideContentsList();
                    ChangePage(page);
                };
                contentsList.Controls.Add(button);

                // increment position
                position += Direction.Down;

                // if first column is full, start the second column
                if (position.Y == contentsList.Height - 2)
                    position = (Program.Width - buttonWidth - 1, 1);
            }
            return contentsList;
        }
    }
}