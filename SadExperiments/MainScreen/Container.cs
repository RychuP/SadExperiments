using SadConsole.UI.Controls;
using SadConsole.UI.Windows;
using SadConsole.UI;
using SadExperiments.Pages;
using SadExperiments.Pages.Sad_Console;
using SadExperiments.Pages.Sad_Canvas;
using SadExperiments.Pages.Primitives;
using SadExperiments.Pages.Go_Rogue;

namespace SadExperiments.MainScreen;

/// <summary>
/// Main program class that coordinates display of pages and headers.
/// </summary>
sealed class Container : ScreenObject
{
    /// <summary>
    /// Lists all available, predefined colors and allows to pick a custom one.
    /// </summary>
    public static ColorPickerPopup ColorPicker { get; } = new();

    /// <summary>
    /// Lists characters available in the default font.
    /// </summary>
    public static CharacterViewer CharacterViewer { get; } = new(1);

    readonly ContentsList _contentsList;
    readonly Header _header;
    Page _currentPage;

    // array of tags added to all pages
    readonly HashSet<Tag> _tags = new();

    // holds pages filtered by tags
    Page[] _filteredPages;

    readonly Page[] _pages =
    {
        new WelcomePage(),
        new AnimatedGlobe(),
        new Donut3dPage(),
        new A_Demo(),
        new LineAlgorithms(),
        new RectangleBisection(),
        new AreaPage(),
        new SplashScreens(),
        new SmoothScrolling(),
        new RectangleManipulation(),
        new StringParser(),
        new FontChanging(),
        new KeyboardAndMouse(),
        new BasicDrawing(),             // tutorial part 1
        new CursorPage(),               // tutorial part 2.1
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

    // singleton pattern as explained at https://csharpindepth.com/articles/singleton
    private static readonly Lazy<Container> lazy = new (() => new Container());

    /// <summary>
    /// Singleton instance of the <see cref="Container"/> class.
    /// </summary>
    public static Container Instance { get => lazy.Value; }

    // static constructor that deals with the static properties
    static Container()
    {
        ColorPicker.FontSize *= 0.9;
        ColorPicker.Center();
        ColorPicker.SelectedColor = Color.White;
        CharacterViewer.Center();
    }

    // private constructor for the singleton use
    private Container()
    {
        // start off with no filters applied
        _filteredPages = _pages;
        ExtractUniqueTags();
        IndexPages();

        // set first page as focused
        _currentPage = _filteredPages[0];
        _currentPage.IsFocused = true;

        // create a header
        _header = new(_currentPage, _filteredPages.Length);

        // instantiate data fields
        _contentsList = new ContentsList(ButtonsWithLinksToPages);
        
        // remove starting console
        Game.Instance.Screen = this;
        Game.Instance.DestroyDefaultStartingConsole();

        // add children 
        Children.Add(_contentsList, _currentPage, _header);
    }

    // sets sequantial index of each page in the filtered array of pages
    void IndexPages()
    {
        int i = 0;
        Array.ForEach(_filteredPages, p => p.Index = i++);
    }

    // keyboard handling of global F1 - F5 keys
    public override void Update(TimeSpan delta)
    {
        var keyboard = Game.Instance.Keyboard;
        if (keyboard.HasKeysPressed)
        {
            if (ColorPicker.IsVisible)
            {
                // keep it seperate
                if (keyboard.IsKeyPressed(Keys.F4))
                    ColorPicker.Hide();
            }
            else if (CharacterViewer.IsVisible)
            {
                // keep it seperate
                if (keyboard.IsKeyPressed(Keys.F5))
                    CharacterViewer.Hide();
            }
            else
            {
                if (keyboard.IsKeyPressed(Keys.F1))
                    PrevPage();
                else if (keyboard.IsKeyPressed(Keys.F2))
                    NextPage();
                else if (keyboard.IsKeyPressed(Keys.F3))
                    ToggleContentsList();
                else if (keyboard.IsKeyPressed(Keys.F4))
                    ColorPicker.Show(true);
                else if (keyboard.IsKeyPressed(Keys.F5))
                    CharacterViewer.Show(true);
            }
        }
        base.Update(delta);
    }

    public void NextPage() => 
        ChangePage(Direction.Right);

    public void PrevPage() => 
        ChangePage(Direction.Left);

    // changes the current page to its neighbour in the direction provided
    void ChangePage(Direction direction)
    {
        if (_contentsList.IsVisible) 
            HideContentsList();

        if (_filteredPages.Length > 0)
        {
            int nextIndex = _currentPage.Index + (direction == Direction.Right ? 1 : -1);
            var page = nextIndex < 0 ? _filteredPages.Last() :
                       nextIndex >= _filteredPages.Length ? _filteredPages.First() :
                                                            _filteredPages[nextIndex];
            ChangeCurrentPage(page);
        }
    }

    // changes the current page to the one provided
    void ChangeCurrentPage(Page page)
    {
        Children.Remove(_currentPage);
        Children.Add(page);
        _currentPage = page;
        _currentPage.IsFocused = true;
        _header.SetHeader(page);
        Children.MoveToTop(_header);
        if (page is IRestartable p)
            p.Restart();
    }

    /// <summary>
    /// Shows/hides contents list.
    /// </summary>
    public void ToggleContentsList()
    {
        if (!_contentsList.IsVisible)
        {
            // hide current page
            Children.MoveToBottom(_currentPage);

            // show contents list
            _contentsList.Show();

            // update header
            _contentsList.Index = _currentPage.Index;
            _header.SetHeader(_contentsList);
        }
        else HideContentsList();
    }

    // hides contents list and shows current page
    void HideContentsList()
    {
        _contentsList.Hide();

        // focus and show current page
        Children.MoveToBottom(_contentsList);
        _currentPage.IsFocused = true;

        // restart page if possible
        if (_currentPage is IRestartable p)
            p.Restart();

        // update header
        _header.SetHeader(_currentPage);
    }

    // extracts all unique tags from the list of filtered pages
    void ExtractUniqueTags()
    {
        _tags.Clear();
        Array.ForEach(_filteredPages, p => _tags.UnionWith(p.Tags));
    }

    /// <summary>
    /// Filters pages by tags provided.
    /// </summary>
    /// <param name="tag1">First <see cref="Tag"/> to filter by.</param>
    /// <param name="tag2">Second <see cref="Tag"/> to filter by.</param>
    public void FilterPagesByTags(Tag? tag1 = null, Tag? tag2 = null)
    {
        // filter pages when both tags are provided
        if (tag1.HasValue && tag2.HasValue)
        {
            _filteredPages = _pages
                .Where(p => p.Tags.Contains(tag1.Value))
                .Where(p => p.Tags.Contains(tag2.Value))
                .ToArray();
        }

        // filter pages when only one tag is provided
        else if (tag1.HasValue || tag2.HasValue)
        {
            Tag? filter = tag1 ?? tag2;
            _filteredPages = _pages
                .Where(p => p.Tags.Contains(filter!.Value))
                .ToArray();
        }

        // no tags -> no filter
        else
            _filteredPages = _pages;

        // save the list of currently available tags based on the filters above
        ExtractUniqueTags();

        // reindex pages
        IndexPages();

        // change the page counter to the number of available filtered pages
        _header.PageCounter.Total = _filteredPages.Length;

        // check if the current page is in the filter results
        if (_filteredPages.Contains(_currentPage))
        {
            _header.PageCounter.ShowIndex(_currentPage.Index);
        }

        // if the filter has any results change current page to the first available
        else if (_filteredPages.Length > 0)
        {
            ChangeCurrentPage(_filteredPages[0]);
        }

        // filter has no results
        else
        {
            // show dummy page index
            _header.PageCounter.ShowIndex(-1);
        }
    }

    // TODO this needs to be transfered to contents list
    ControlsConsole ButtonsWithLinksToPages
    {
        get
        {
            var contentsList = new ControlsConsole(Program.Width, Program.Height - TagFilters.MinimizedHeight);
            contentsList.Position = (0, TagFilters.MinimizedHeight);
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
                    ChangeCurrentPage(page);
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