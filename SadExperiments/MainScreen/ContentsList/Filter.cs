using static SadConsole.UI.Controls.ListBox;

namespace SadExperiments.MainScreen;

/// <summary>
/// Page filter based on tags for the use of <see cref="ContentsList"/>.
/// </summary>
class Filter : ScreenSurface
{
    #region Constants
    public const int MinimizedHeight = 4;
    #endregion Constants

    #region Fields
    readonly TagSelector _tag1Selector;
    readonly TagSelector _tag2Selector;
    readonly SortMethodSelector _sortOrderSelector;
    #endregion Fields

    #region Constructors
    public Filter() : base(Program.Width, MinimizedHeight, Program.Width, Program.Height / 2)
    {
        Surface.SetDefaultColors(Header.FGColor, Header.BGColor);

        // calculate tag selector dimensions
        int spacer = 4;
        int noOfControls = 3;
        int height = Surface.Height - 1;
        int width = (Surface.Width - noOfControls * spacer) / noOfControls;

        // create tag selectors
        Point position = (spacer, 1);
        _tag1Selector = new(position, width, height);
        position += (width + spacer, 0);
        _tag2Selector = new(position, width, height);
        position += (width + spacer, 0);
        _sortOrderSelector = new(position, width, height);

        // add tag selectors to children
        Children.Add(_tag1Selector, _tag2Selector, _sortOrderSelector);

        // register event handlers
        _tag1Selector.ListBox.SelectedItemExecuted += (o, e) => FilterPages();
        _tag1Selector.ClearButton.Click += (o, e) => FilterPages();
        _tag2Selector.ListBox.SelectedItemExecuted += (o, e) => FilterPages();
        _tag2Selector.ClearButton.Click += (o, e) => FilterPages();
        if (Game.Instance.Screen is Container container)
            container.PageChanged += Container_OnPageChanged;
    }
    #endregion Constructors

    #region Properties
    /// <summary>
    /// Checks if the filter height is equal to MinimizedHeight.
    /// </summary>
    public bool IsMinimized =>
        Surface.ViewHeight == MinimizedHeight;
    #endregion Properties

    #region Methods
    /// <summary>
    /// Collapses top section of the console and hides tag selection.
    /// </summary>
    public void Minimize()
    {
        if (!IsMinimized)
        {
            Surface.ViewHeight = MinimizedHeight;
            foreach (var selector in Children)
                (selector as OptionSelector)?.Minimize();
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
            foreach (var selector in Children)
                (selector as OptionSelector)?.Maximize();
        }
    }

    protected virtual void ListBox_OnSelectedItemExecuted(object? sender, SelectedItemEventArgs args)
    {
        
    }

    void FilterPages()
    {
        var tag1 = (Tag?)_tag1Selector.ListBox.SelectedItem;
        var tag2 = (Tag?)_tag2Selector.ListBox.SelectedItem;
        Container.Instance.FilterPagesByTags(tag1, tag2);
    }

    // minimize on pressing esc key while filter is displayed
    public override void Update(TimeSpan delta)
    {
        var keyboard = Game.Instance.Keyboard;
        if (keyboard.HasKeysPressed)
        {
            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                Minimize();
            }
        }

        base.Update(delta);
    }

    // minimizes filter on page changed
    protected virtual void Container_OnPageChanged(object? sender, EventArgs args)
    {
        Minimize();
    }
    #endregion Methods
}