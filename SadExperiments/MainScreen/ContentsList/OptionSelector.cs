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

        // create a button for clearing text
        ClearButton = new ClearButton();
        ClearButton.Position = (Width - ClearButton.Width, 0);
        Controls.Add(ClearButton);

        // register event handlers
        ListBox.SelectedItemExecuted += (o, e) => ShowSelectedOption();
        TextBox.EditModeExit += (o, e) => ShowSelectedOption();
        ClearButton.Click += (o, e) =>
        {
            TextBox.Text = string.Empty;
            ListBox.SelectedItem = null;
        };
    }
    #endregion Constructors

    #region Properties
    /// <summary>
    /// Text box that displays the selected tag.
    /// </summary>
    public TextBox TextBox { get; init; }

    /// <summary>
    /// List available page tags.
    /// </summary>
    public ListBox ListBox { get; init; }

    /// <summary>
    /// Button for clearing selected item.
    /// </summary>
    public ClearButton ClearButton { get; init; }
    #endregion Properties

    #region Methods
    /// <summary>
    /// Shows list box.
    /// </summary>
    public void Maximize() =>
        Surface.ViewHeight = Height;

    /// <summary>
    /// Hides list box.
    /// </summary>
    public void Minimize() =>
        Surface.ViewHeight = MinimizedHeight;

    // replaces textbox text with listbox selected item
    void ShowSelectedOption() =>
        TextBox.Text = ListBox.SelectedItem != null ? ListBox.SelectedItem.ToString() : string.Empty;
    #endregion Methods
}