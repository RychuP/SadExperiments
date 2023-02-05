using SadExperiments.UI;
using SadConsole.Components;
using static SadExperiments.MainScreen.Container;

namespace SadExperiments.MainScreen;

/// <summary>
/// Window shown at the top of the screen with information about currently loaded page.
/// </summary>
class Header : Console
{
    #region Constants
    /// <summary>
    /// Header foreground color.
    /// </summary>
    public static readonly Color FGColor = Color.White;

    /// <summary>
    /// Header background color.
    /// </summary>
    public static readonly Color BGColor = Color.AnsiBlackBright;

    /// <summary>
    /// Default, minimized view height of the header (no mouse over).
    /// </summary>
    public const int MinimizedViewHeight = 2;
    #endregion Constants

    #region Fields
    // height of the current page description content that the header expands to on mouse over
    int _contentViewHeight = MinimizedViewHeight;

    // console that displays tags associated with the current page
    readonly Buttons _tagButtons;
    #endregion Fields

    #region Constructors
    public Header() : base(Program.Width, MinimizedViewHeight, Program.Width, Program.Height)
    {
        // set colors
        Surface.SetDefaultColors(FGColor, BGColor);

        // setup cursor
        Cursor.IsVisible = false;
        Cursor.UseStringParser = true;

        // create page counter
        PageCounter = new PageCounter();
        PageCounter.Position = (Surface.Width - PageCounter.Surface.Width, 0);

        // create tag buttons
        _tagButtons = new Buttons(1, 1);

        // add children
        Children.Add(PageCounter, _tagButtons);
    }
    #endregion Constructors

    #region Properties
    /// <summary>
    /// Page counter displayed in the top right corner of the header.
    /// </summary>
    public PageCounter PageCounter { get; init; }

    /// <summary>
    /// True if the view height is equal to <see cref="MinimizedViewHeight"/>.
    /// </summary>
    public bool IsMinimized =>
        Surface.ViewHeight == MinimizedViewHeight;
    #endregion Properties

    #region Methods
    /// <summary>
    /// Expands the view height of the header to the size of the current page description content.
    /// </summary>
    public void Maximize()
    {
        if (Root.CurrentPage is not ContentsList)
        {
            Surface.ViewHeight = _contentViewHeight;
            _tagButtons.IsVisible = true;
        }
    }

    /// <summary>
    /// Reduces the view height of the header to <see cref="MinimizedViewHeight"/>.
    /// </summary>
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

    public void Container_OnPageChanged(object? sender, EventArgs args)
    {
        Surface.Clear();
        Page page = Root.CurrentPage;

        // display main info about the page
        Cursor
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
        _tagButtons.Position = Cursor.Position;

        // prepare buttons console for the new tags
        _tagButtons.Resize(Surface.Width - Cursor.Position.X, 1, true);
        _tagButtons.Controls.Clear();
        _tagButtons.CurrentRow = 0;

        // add all tags as buttons
        foreach (var tag in page.Tags)
            _tagButtons.AddButton($"{tag}");

        // change content height field according to the content of the page's header data
        _contentViewHeight = Cursor.Position.Y + _tagButtons.Height + 1;

        // set current view height to minimized height (default state)
        Minimize();
    }
    #endregion Methods

    #region Types
    // made for the sole purpose of handling mouse exit to the right of this console
    // where header can't catch it with its own onMouseExit()
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
    #endregion Types
}