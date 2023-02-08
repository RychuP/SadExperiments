using SadConsole.UI;
using SadConsole.UI.Controls;
using SadExperiments.UI;
using SadExperiments.UI.Controls;
using System.Text;

namespace SadExperiments.MainScreen;

/// <summary>
/// Buttons with links to pages.
/// </summary>
class PageLinks : ControlsConsole
{
    #region Constants
    const int ButtonWidth = ColumnWidth - ButtonMargin * 2;
    // total number of rows that need to be substracted from current display height to form the height of a column
    const int VerticalPadding = 4;
    // margin for a button within a column 
    const int ButtonMargin = 1;
    // column width for the buttons
    const int ColumnWidth = Program.Width / 2;
    // display height when the console is minimized
    const int MinimizedHeight = Program.Height - Filter.MaximizedHeight;
    // display height when the console is maximized
    const int MaximizedHeight = Program.Height - Filter.MinimizedHeight;
    #endregion Constants
    
    #region Fields
    readonly DotNavigation _dotNav = new();
    #endregion Fields

    #region Constructors
    public PageLinks() : base(Program.Width, MaximizedHeight)
    {
        Position = (0, Filter.MinimizedHeight);
        Surface.UsePrintProcessor = true;

        // add dot navigation
        Children.Add(_dotNav);

        // register event handlers
        _dotNav.SelectedDotChanged += DotNav_OnSelectedDotChanged;
        if (Game.Instance.Screen is Container container)
            container.PageListChanged += Container_OnPageListChanged;
    }
    #endregion Constructors

    #region Properties
    bool IsMinimized =>
        Position == (0, Filter.MaximizedHeight);

    int ButtonsPerColumn =>
        (IsMinimized ? MinimizedHeight : MaximizedHeight) - VerticalPadding;
    #endregion Properties

    #region Methods
    // registers filter event handlers
    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is not ContentsList contentsList) return;
        contentsList.Filter.Minimized += (o, e) => Maximize();
        contentsList.Filter.Maximized += (o, e) => Minimize();
        base.OnParentChanged(oldParent, newParent);
    }

    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        if (state.IsOnScreenObject)
        {
            if (state.Mouse.ScrollWheelValueChange != 0)
            {
                if (state.Mouse.ScrollWheelValueChange < 0)
                    _dotNav.Value -= 1;
                else
                    _dotNav.Value += 1;
            }
        }

        return base.ProcessMouse(state);
    }

    void Maximize()
    {
        Position = (0, Filter.MinimizedHeight);
        ResetLayout();
    }

    void Minimize()
    {
        Position = (0, Filter.MaximizedHeight);
        ResetLayout();
    }

    void ResetLayout()
    {
        RepositionButtons();
        ResetDotNav();
    }

    void ResetDotNav()
    {
        int dotCount = (int)Math.Ceiling((decimal)Controls.Count / ButtonsPerColumn) - 1;
        dotCount = dotCount < 1 ? 1 : dotCount;

        // remove the event handling so as not to trigger the unnecessary button repositioning
        _dotNav.SelectedDotChanged -= DotNav_OnSelectedDotChanged;
        // update the dot count (this will also set dotNav.Value to 0)
        _dotNav.Maximum = dotCount;
        // reinstate the event handling
        _dotNav.SelectedDotChanged += DotNav_OnSelectedDotChanged;

        // update dotNav position
        int y = (ButtonsPerColumn + 3) * FontSize.Y - FontSize.Y / 2;
        int x = (WidthPixels / 2 - _dotNav.WidthPixels / 2);
        _dotNav.Position = (x, y);
    }

    void RepositionButtons(int offsetX = 0)
    {
        int overflowY = ButtonsPerColumn + 2;

        // ??? for some reason button repositioning on its own doesn't refresh the surface
        // until a mouse goes over any button or, like in this case, buttons are removed 
        // and inserted again
        var controls = Controls.ToArray();
        Controls.Clear();

        // group all buttons in columns of a size based on the given height
        Point position = (ButtonMargin - offsetX, 1);
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i].Position = position;
            Controls.Add(controls[i]);
            position += Direction.Down;
            if (position.Y == overflowY)
            {
                int x = position.X + ColumnWidth;
                position = (x, 1);
            }
        }
    }

    void DotNav_OnSelectedDotChanged(object? o, EventArgs e)
    {
        int offsetX = _dotNav.Value * ColumnWidth;
        RepositionButtons(offsetX);
    }

    public void PrintDebugInfo(object? o, EventArgs e)
    {
        //var sb = new StringBuilder();
        //foreach (var control in _dotNav.Controls)
        //{
        //    if (control is DotButton dot)
        //    {
        //        sb.Append($"{dot.State.HasFlag(ControlStates.Selected)}, ");
        //    }
        //}
        //Surface.Print(Point.Zero, sb.ToString().Align(HorizontalAlignment.Left, Width));

        if (o is DotButton b)
        {
            Surface.Print(Point.Zero, b.State.ToString().Align(HorizontalAlignment.Left, Width));
        }
    }

    // creates buttons with links to pages
    void Container_OnPageListChanged(object? sender, EventArgs args)
    {
        if (sender is not Container container) return;

        Controls.Clear();
        foreach (Page page in container.PageList)
        {
            // create a new button with a link to the given page
            var button = new Button(ButtonWidth, 1)
            {
                Text = page.Title,
                UseMouse = true,
                UseKeyboard = false,
            };
            button.Click += (o, e) =>
                container.CurrentPage = page;

            Controls.Add(button);
        }

        ResetLayout();
    }
    #endregion Methods
}