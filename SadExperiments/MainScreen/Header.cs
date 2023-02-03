using SadConsole.Components;
using SadExperiments.UI;

namespace SadExperiments.MainScreen;

class Header : ScreenSurface
{
    /// <summary>
    /// Default, minimized view height of the header (no mouse over).
    /// </summary>
    public const int MinimizedViewHeight = 2;

    // view height that includes all current content shown on mouse over
    int _contentViewHeight = MinimizedViewHeight;

    /// <summary>
    /// Page counter displayed in the top right corner of the header.
    /// </summary>
    public PageCounter PageCounter { get; init; }

    readonly Cursor _cursor;
    readonly Buttons _tagButtons;
    Page _currentPage;

    /// <summary>
    /// Window shown at the top of the screen with information about currently loaded page.
    /// </summary>
    /// <param name="page">Initial page shown when the program starts.</param>
    /// <param name="pageCount">Total number of pages used by <see cref="MainScreen.PageCounter"/>.</param>
    public Header(Page page, int pageCount) : base(Program.Width, MinimizedViewHeight, Program.Width, Program.Height)
    {
        // set colors
        Surface.SetDefaultColors(Color.White, Color.DarkGray.GetDarker(), false);

        // add page counter
        PageCounter = new PageCounter(pageCount) { Parent = this };
        PageCounter.Position = (Surface.Width - PageCounter.Surface.Width, 0);

        // add cursor
        _cursor = new Cursor()
        {
            IsVisible = false,
            UseStringParser = true,
        };
        SadComponents.Add(_cursor);

        // add tag buttons
        _tagButtons = new Buttons(1, 1) { Parent = this };

        // set initial data based on the first page provided
        _currentPage = page;
        SetHeader(page);
    }

    public void SetHeader(Page page)
    {
        Surface.Clear();

        // set page counter index
        PageCounter.ShowIndex(page.Index);

        // display main info about the page
        _cursor
            // title
            .Move(new Point(1, 0)).Print($"[c:r f:yellow]{page.Title.ToUpper()}")
            .NewLine().Right(1)
            // summary
            .Print(page.Summary)
            .NewLine().Down(1).Right(1)
            // submitter
            .Print($"[c:r f:yellow]Submitted by[c:undo]: {page.Submitter}")
            .NewLine().Down(1).Right(1)
            // tags title
            .Print($"[c:r f:yellow]Tags[c:undo]: ");

        // place the buttons console at the end of the 'Tags: ' text
        _tagButtons.Position = _cursor.Position;

        // prepare buttons console for the new tags
        _tagButtons.Resize(Surface.Width - _cursor.Position.X, 1, true);
        _tagButtons.Controls.Clear();
        _tagButtons.CurrentRow = 0;

        // add all tags as buttons
        foreach (var tag in page.Tags)
            _tagButtons.AddButton($"{tag}");

        // change content height field according to the content of the page's header data
        _contentViewHeight = _cursor.Position.Y + _tagButtons.Height + 1;

        // set current view height to minimized height (default state)
        Minimize();

        _currentPage = page;
    }

    public void Maximize()
    {
        if (_currentPage is not ContentsList)
        {
            Surface.ViewHeight = _contentViewHeight;
            _tagButtons.IsVisible = true;
        }
    }

    public bool IsMinimized =>
        Surface.ViewHeight == MinimizedViewHeight;

    public void Minimize()
    {
        Surface.ViewHeight = MinimizedViewHeight;
        _tagButtons.IsVisible = false;
    }

    protected override void OnMouseEnter(MouseScreenObjectState state)
    {
        Maximize();
        base.OnMouseEnter(state);
    }

    protected override void OnMouseExit(MouseScreenObjectState state)
    {
        if (!state.IsOnScreenObject)
            Minimize();
        base.OnMouseExit(state);
    }

    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        if (state.IsOnScreenObject && IsMinimized)
            Maximize();
        return base.ProcessMouse(state);
    }

    // made for the sole purpose of handling mouse exit to the right of this console where header can't catch it
    class Buttons : HorizontalButtonsConsole
    {
        public Buttons(int w, int h) : base(w, h) { }

        protected override void OnMouseExit(MouseScreenObjectState state)
        {
            if (Parent is Header header)
            {
                // create the mouse state for the parent and check if the mouse is still on it
                var parentState = new MouseScreenObjectState(header, state.Mouse);
                if (!parentState.IsOnScreenObject)
                    header.Minimize();
            }
            base.OnMouseExit(state);
        }
    }
}