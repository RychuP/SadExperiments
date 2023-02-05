using SadConsole.UI;
using SadConsole.UI.Controls;
using static SadExperiments.MainScreen.Container;

namespace SadExperiments.MainScreen;

class TagSelector : ControlsConsole
{
    const int MinimizedHeight = 2;
    public TextBox TextBox { get; init; }
    readonly ListBox _tagList;

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

    public void RegisterEventHandlers()
    {
        Root.TagsChanged += Container_OnTagsChanged;
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

    public void Container_OnTagsChanged(object? sender, EventArgs args)
    {
        _tagList.Items.Clear();
        foreach (var tag in Root.Tags)
            _tagList.Items.Add(tag.ToString());
    }
}