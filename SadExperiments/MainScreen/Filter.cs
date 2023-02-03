using SadConsole.UI.Controls;
using SadConsole.UI;

namespace SadExperiments.MainScreen;

/// <summary>
/// Page filter based on tags for the use of <see cref="ContentsList"/>.
/// </summary>
class Filter : ScreenSurface
{
    public const int MinimizedHeight = 4;

    readonly TagSelector _tag1Selector;
    readonly TagSelector _tag2Selector;
    readonly TagSelector _sortOrderSelector;

    public Filter() : base(Program.Width, 4, Program.Width, Program.Height / 2)
    {
        Surface.SetDefaultColors(Header.FGColor, Header.BGColor);

        // calculate tag selector dimensions
        int spacer = 4;
        int noOfControls = 3;
        int height = Surface.Height - 1;
        int width = (Surface.Width - noOfControls * spacer) / noOfControls;

        // create tag selectors
        Point position = (spacer, 1);
        _tag1Selector = new TagSelector("Filter by:", position, width, height);
        position += (width + spacer, 0);
        _tag2Selector = new TagSelector("Filter by:", position, width, height);
        position += (width + spacer, 0);
        _sortOrderSelector = new TagSelector("Sort method:", position, width, height);

        // add tag selectors to children
        Children.Add(_tag1Selector, _tag2Selector, _sortOrderSelector);

        // add edit mode handling to text boxes
        foreach (var child in Children)
        {
            if (child is TagSelector tagSelector)
            {
                tagSelector.TextBox.EditModeEnter += TextBox_OnEditModeEnter;
                tagSelector.TextBox.EditModeExit += TextBox_OnEditModeExit;
            }
        }
    }

    /// <summary>
    /// True if the window is collapsed and doesn't show tag selection buttons any more.
    /// </summary>
    public bool IsMinimized =>
        Surface.ViewHeight == MinimizedHeight;

    /// <summary>
    /// Collapses top section of the console and hides tag selection.
    /// </summary>
    public void Minimize()
    {
        if (!IsMinimized)
        {
            Surface.ViewHeight = MinimizedHeight;
            foreach (var child in Children)
                if (child is TagSelector tagSelector)
                    tagSelector.HideTagList();
        }
    }

    /// <summary>
    /// Expands top section of the console to show tag selection.
    /// </summary>
    public void Maximize()
    {
        if (IsMinimized)
        {
            Surface.ViewHeight = Surface.Height;
            foreach (var child in Children)
                if (child is TagSelector tagSelector)
                    tagSelector.ShowTagList();
        }
    }

    // default behaviour when the text box is clicked and enters edit mode
    protected virtual void TextBox_OnEditModeEnter(object? o, EventArgs e)
    {
        Maximize();
    }

    // default behaviour when the ESC is pressed and the text box leaves edit mode
    protected virtual void TextBox_OnEditModeExit(object? o, EventArgs e)
    {
        //bool otherTextBoxIsFocused = false;
        //foreach (var child in Children)
        //    if (child is TagSelector tagSelector)
        //        if (tagSelector.TextBox.IsFocused)
        //            otherTextBoxIsFocused = true;
        //if (!otherTextBoxIsFocused)
            Minimize();
    }
}

class TagSelector : ControlsConsole
{
    const int MinimizedHeight = 2;
    public TextBox TextBox { get; init; }
    ListBox _tagList;

    public TagSelector(string title, Point position, int w, int h) : base(w, MinimizedHeight, w, h)
    {
        // surface setup
        Surface.SetDefaultColors(Header.FGColor, Header.BGColor);
        Position = position;

        // print title
        Surface.Print(Point.Zero, title);

        // create a text box
        TextBox = new TextBox(Width);
        TextBox.Position = (0, 1);
        Controls.Add(TextBox);
        TextBox.EditModeEnter += TextBox_OnEditModeEnter;
        TextBox.EditModeExit += TextBox_OnEditModeExit;

        // draw an outline for the list box
        int borderHeight = Height - MinimizedHeight;
        var border = new Rectangle(0, MinimizedHeight, Width, borderHeight);
        Surface.DrawRectangle(border);

        // create a list box for the tags
        _tagList = new ListBox(Width - 2, Height - MinimizedHeight - 2);
        _tagList.Position = (1, MinimizedHeight + 1);
        Controls.Add(_tagList);
    }

    void SetTags(Tag[] tags)
    {
        _tagList.Items.Clear();
        foreach (var tag in tags)
            _tagList.Items.Add(tag.ToString());
    }

    public void ShowTagList()
    {
        Surface.ViewHeight = Height;
    }
        

    public void HideTagList()
    {
        Surface.ViewHeight = MinimizedHeight;
    }

    // default behaviour when the text box is clicked and enters edit mode
    protected virtual void TextBox_OnEditModeEnter(object? o, EventArgs e)
    {
        
    }

    // default behaviour when the ESC is pressed and the text box leaves edit mode
    protected virtual void TextBox_OnEditModeExit(object? o, EventArgs e)
    {
        
    }
}