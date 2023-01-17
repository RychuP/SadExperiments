using SadConsole.UI.Controls;
using SadConsole.UI;
using SadExperiments.Pages;
namespace SadExperiments.MainScreen;

internal class Container : ScreenObject
{
    readonly Header _header;
    readonly Page _contentsList;
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
        // set first page as focused
        var firstPage = _pages[0];
        firstPage.IsFocused = true;

        // create a header
        _header = new(firstPage, _pages.Length);

        // create contents list
        _contentsList = new ContentsList(GetContentsList());

        // remove starting console
        Game.Instance.Screen = this;
        Game.Instance.DestroyDefaultStartingConsole();

        // add children
        Children.Add(_header);
        Children.Add(firstPage);

        // set page indices
        int i = 0;
        Array.ForEach(_pages, p => p.Index = i++);
    }

    Page Page
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

    public void NextPage() => ChangePage(Direction.Right);

    public void PrevPage() => ChangePage(Direction.Left);

    void ChangePage(Direction direction)
    {
        int nextIndex = Page.Index + (direction == Direction.Right ? 1 : -1);
        var page = nextIndex < 0              ? _pages.Last() :
                   nextIndex >= _pages.Length ? _pages.First() :
                                                _pages[nextIndex];
        SetPage(page);
    }

    public void SetPage(Page page)
    {
        Page = page;
        Page.IsFocused = true;
        _header.SetHeader(page);
        if (page is IRestartable p) p.Restart();
    }

    public void ShowContentsList()
    {
        SetPage(_contentsList!);
    }

    ControlsConsole GetContentsList()
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
            button.Click += (o, e) => SetPage(page);
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