using SadConsole.UI;
using SadConsole.UI.Controls;
using static SadConsole.UI.Controls.ListBox;

namespace SadExperiments.MainScreen;

class OptionSelector : ControlsConsole
{
    #region Constants
    const int MinimizedHeight = 2;
    #endregion Constants

    #region Constructors
    public OptionSelector(string title, Point position, int w, int h) : base(w, MinimizedHeight, w, h)
    {
        // surface setup
        Surface.SetDefaultColors(Header.FGColor, Header.BGColor);
        Position = position;

        // print title
        Surface.Print(Point.Zero, title);

        // create a text box
        TextBox = new(Width);
        TextBox.Position = (0, 1);
        Controls.Add(TextBox);

        // draw an outline for the list box
        int borderHeight = Height - MinimizedHeight;
        var border = new Rectangle(0, MinimizedHeight, Width, borderHeight);
        Surface.DrawRectangle(border);

        // create a list box for the tags
        ListBox = new(Width - 2, Height - MinimizedHeight - 2);
        ListBox.Position = (1, MinimizedHeight + 1);
        ListBox.SingleClickItemExecute = true;
        Controls.Add(ListBox);

        // register event handlers
        TextBox.EditModeEnter += TextBox_OnEditModeEnter;
        TextBox.EditModeExit += (o, e) => ShowSelectedOption();
        //ListBox.SelectedItemChanged += ListBox_SelectedItemChanged;
        ListBox.SelectedItemExecuted += (o, e) => ShowSelectedOption();

        // create a button for clearing text
        ClearButton = new ClearButton();
        ClearButton.Position = (Width - ClearButton.Width, 0);
        ClearButton.Click += (o, e) =>
        {
            TextBox.Text = string.Empty;
            ListBox.SelectedItem = null;
        };
        Controls.Add(ClearButton);
    }
    #endregion Constructors

    #region Properties
    // checks if the surface height is equal MinimizedHeight
    bool IsMinimized =>
        Surface.ViewHeight == MinimizedHeight;

    protected TextBox TextBox { get; init; }
    public ListBox ListBox { get; init; }
    public ClearButton ClearButton { get; init; }
    #endregion Properties

    #region Methods
    // shows list box
    public void Maximize()
    {
        if (IsMinimized)
            Surface.ViewHeight = Height;
    }

    // hides list box
    public void Minimize()
    {
        if (!IsMinimized)
            Surface.ViewHeight = MinimizedHeight;
    }

    void ShowSelectedOption()
    {
        TextBox.Text = ListBox.SelectedItem != null ? ListBox.SelectedItem.ToString() : string.Empty;
    }

    // default behaviour when the text box is clicked and enters edit mode
    protected virtual void TextBox_OnEditModeEnter(object? sender, EventArgs args)
    {
        if (Parent is Filter filter)
            filter.Maximize();
    }

    protected virtual void TextBox_OnEditeModeExit(object? sender, EventArgs args)
    {
        ShowSelectedOption();
    }

    protected virtual void ListBox_SelectedItemChanged(object? sender, SelectedItemEventArgs args)
    {
        ShowSelectedOption();
    }
    #endregion Methods
}