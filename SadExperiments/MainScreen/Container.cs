using SadConsole.UI.Windows;
using SadExperiments.Pages;
using System.Collections.Immutable;
using System.Diagnostics;

namespace SadExperiments.MainScreen;

/// <summary>
/// Root screen object that coordinates display of pages and headers.
/// </summary>
[DebuggerDisplay("Container")]
sealed class Container : ScreenObject
{
    #region Fields
    // list of pages
    readonly ContentsList _contentsList;

    // header with info about the current page
    readonly Header _header;

    // currently displayed page
    Page _currentPage = new Template();

    // previous page 
    Page? _prevPage = null;

    // singleton pattern as explained at https://csharpindepth.com/articles/singleton
    static readonly Lazy<Container> s_lazy = new(() => new Container());

    // window with a list of available colors and a color composer
    readonly ColorPickerPopup _colorPicker = new();

    // window with characters available in the default IBM font
    readonly CharacterViewer _characterViewer = new();

    // unique tags from the list of filtered pages
    List<Tag> _tags = new();

    // pages filtered by tags
    Page[] _filteredPages = Array.Empty<Page>();

    // all available pages
    readonly Page[] _pages =
    {
        //new Test(),
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
        //new Fluid(),                  // doesn't quite work yet
        //new Crash(),                  // this page crashes the engine
    };

    readonly TagSorter _tagSorter = new();
    readonly DefaultPageSorter _defaultPageSorter;
    readonly DatePageSorter _datePageSorter = new();
    #endregion Fields

    #region Constructors
    // private constructor for the singleton use
    private Container()
    {
        // remove starting console
        Game.Instance.Screen = this;
        Game.Instance.DestroyDefaultStartingConsole();

        // setup color picker
        _colorPicker.SelectedColor = Color.White;
        _colorPicker.FontSize *= 0.9;
        _colorPicker.Center();

        // create contents list and header
        _contentsList = new();
        _header = new();

        // add consoles to children 
        Children.Add(CurrentPage, _header);

        _defaultPageSorter = new(_pages);
    }
    #endregion Constructors

    #region Properties
    /// <summary>
    /// Singleton instance of the <see cref="Container"/> class.
    /// </summary>
    public static Container Instance => s_lazy.Value;

    /// <summary>
    /// Page currently selected for the display.
    /// </summary>
    public Page CurrentPage
    {
        get => _currentPage;
        set
        {
            if (value != _contentsList && !PageList.Contains(value))
                throw new ArgumentException("Page has to be either in the PageList or be a contents list.");

            // handle fields
            _prevPage = _currentPage;
            _currentPage = value;
            _currentPage.IsFocused = true;

            // trigger events
            OnPageChanged();
        }
    }

    public Page[] PageList
    {
        get => _filteredPages;
        private set
        {
            _filteredPages = value;

            // trigger events
            OnPageListChanged();
            OnTagsChanged();
        }
    }

    /// <summary>
    /// Filtered page tags.
    /// </summary>
    public List<Tag> Tags => _tags;
    #endregion Properties

    #region Methods
    /// <summary>
    /// Keyboard handling of global F1 - F5 keys.
    /// </summary>
    public override void Update(TimeSpan delta)
    {
        var keyboard = Game.Instance.Keyboard;
        if (keyboard.HasKeysPressed)
        {
            if (_colorPicker.IsVisible)
            {
                if (keyboard.IsKeyPressed(Keys.F4))
                    _colorPicker.Hide();
            }
            else if (_characterViewer.IsVisible)
            {
                if (keyboard.IsKeyPressed(Keys.F5))
                    _characterViewer.Hide();
            }
            else
            {
                if (keyboard.IsKeyPressed(Keys.F1))
                    PrevPage();
                else if (keyboard.IsKeyPressed(Keys.F2))
                    NextPage();
                else if (keyboard.IsKeyPressed(Keys.F3))
                {
                    if (CurrentPage == _contentsList && _prevPage != null)
                        CurrentPage = _prevPage;
                    else
                        CurrentPage = _contentsList;
                }
                else if (keyboard.IsKeyPressed(Keys.F4))
                    _colorPicker.Show(true);
                else if (keyboard.IsKeyPressed(Keys.F5))
                    _characterViewer.Show(true);
            }
        }
        base.Update(delta);
    }

    public void SortPages(SortMethod method = SortMethod.Default)
    {
        var pages = PageList.ToList();
        switch (method)
        {
            case SortMethod.Alphabetical:
                pages.Sort();
                break;

            case SortMethod.Date:
                pages.Sort(_datePageSorter); 
                break;

            case SortMethod.Default:
                pages.Sort(_defaultPageSorter);
                break;
        }
        PageList = pages.ToArray();
    }

    /// <summary>
    /// Filters pages by tags.
    /// </summary>
    /// <param name="tag1">First <see cref="Tag"/> to filter by.</param>
    /// <param name="tag2">Second <see cref="Tag"/> to filter by.</param>
    public void FilterPagesByTags(Tag? tag1 = null, Tag? tag2 = null)
    {
        // filter pages when both tags are provided
        if (tag1.HasValue && tag2.HasValue)
        {
            PageList = _pages
                .Where(p => p.Tags.Contains(tag1.Value))
                .Where(p => p.Tags.Contains(tag2.Value))
                .ToArray();
        }

        // filter pages when only one tag is provided
        else if (tag1.HasValue || tag2.HasValue)
        {
            Tag? filter = tag1 ?? tag2;
            PageList = _pages
                .Where(p => p.Tags.Contains(filter!.Value))
                .ToArray();
        }

        // no tags -> no filter
        else
            PageList = _pages;
    }

    public void Init()
    {
        PageList = _pages;
        CurrentPage = PageList[0];
    }

    // changes the current page to the one with a higher index (+1) wrapping if needed
    void NextPage() => 
        ChangePage(Direction.Right);

    // changes the current page to the one with a lower index (-1) wrapping if needed
    void PrevPage() => 
        ChangePage(Direction.Left);

    // changes the current page to its neighbour in the direction provided
    void ChangePage(Direction direction)
    {
        if (PageList.Length > 0)
        {
            Page? page = CurrentPage == _contentsList ? _prevPage : CurrentPage;
            if (page != null)
            {
                int nextIndex = page.Index + (direction == Direction.Right ? 1 : -1);
                page = nextIndex < 0                 ? PageList.Last() :
                       nextIndex >= PageList.Length  ? PageList.First() :
                                                       PageList[nextIndex];
                CurrentPage = page;
            }
        }
    }

    // sets sequantial index of each page in the filtered array of pages
    void IndexPages()
    {
        int i = 0;
        Array.ForEach(_filteredPages, p => p.Index = i++);
    }

    // extracts all unique tags from the list of filtered pages
    void ExtractUniqueTags()
    {
        HashSet<Tag> tags = new();
        Array.ForEach(_filteredPages, p => tags.UnionWith(p.Tags));
        _tags = tags.ToList();
        _tags.Sort(_tagSorter);
    }

    void OnPageChanged()
    {
        // remove the prev page and add the new one to children
        Children.Remove(_prevPage);
        Children.Add(_currentPage);
        Children.MoveToTop(_header);

        // restart page if possible
        if (_currentPage is IRestartable page)
            page.Restart();

        PageChanged?.Invoke(this, EventArgs.Empty);
    }

    void OnPageListChanged()
    {
        // extract tags and reindex pages
        ExtractUniqueTags();
        IndexPages();

        // check whether to leave the prev page field as it was or assign to it a new value 
        _prevPage = (PageList.Length == 0)          ? null :
                    (!PageList.Contains(_prevPage)) ? PageList[0] :
                                                      _prevPage;

        // invoke event
        PageListChanged?.Invoke(this, EventArgs.Empty);
    }

    void OnTagsChanged()
    {
        TagsChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion Methods

    #region Events
    public event EventHandler? PageChanged;

    public event EventHandler? PageListChanged;

    public event EventHandler? TagsChanged;
    #endregion Events
}

class TagSorter : IComparer<Tag>
{
    public int Compare(Tag t1, Tag t2)
    {
        string t1Name = t1.ToString();
        string t2Name = t2.ToString();
        return t1Name.CompareTo(t2Name);
    }
}

class DefaultPageSorter : IComparer<Page>
{
    readonly Page[] _pages;

    public DefaultPageSorter(Page[] pages)
    {
        _pages = pages;
    }

    public int Compare(Page? p1, Page? p2)
    {
        int index1 = Array.FindIndex(_pages, p => p == p1);
        int index2 = Array.FindIndex(_pages, p => p == p2);
        return index1.CompareTo(index2);
    }
}

class DatePageSorter : IComparer<Page>
{
    public int Compare(Page? p1, Page? p2)
    {
        return p1!.Date.CompareTo(p2!.Date);
    }
}