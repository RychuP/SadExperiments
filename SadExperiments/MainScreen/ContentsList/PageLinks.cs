using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadExperiments.MainScreen;

/// <summary>
/// Buttons with links to pages.
/// </summary>
class PageLinks : ControlsConsole
{
    const int VerticalPadding = 4;
    const int HorizontalPadding = 2;
    const int MinimizedHeight = Program.Height - Filter.MaximizedHeight;
    const int MaximizedHeight = Program.Height - Filter.MinimizedHeight;
    const int ColumnWidth = Program.Width / 2;
    readonly ScrollBar _scrollBar;

    public PageLinks(Filter filter) : base(Program.Width, MaximizedHeight)
    {
        Position = (0, Filter.MinimizedHeight);

        _scrollBar = new(Orientation.Horizontal, Width - HorizontalPadding);
        Controls.Add(_scrollBar);

        // register event handlers
        filter.Minimized += (o, e) => Maximize();
        filter.Maximized += (o, e) => Minimize();
        if (Game.Instance.Screen is Container container)
            container.PageListChanged += Container_OnPageListChanged;

        _scrollBar.ValueChanged += (o, e) =>
        {
            Surface.Print(Point.Zero, $"Value: {_scrollBar.Value}, Max: {_scrollBar.Maximum}");
            int x = _scrollBar.Value * ColumnWidth;
            Surface.ViewPosition = Surface.ViewPosition.WithX(x);
        };
    }

    public void Maximize()
    {
        Position = (0, Filter.MinimizedHeight);
        ResizeAndRepositionButtons(MaximizedHeight);
    }

    public void Minimize()
    {
        Position = (0, Filter.MaximizedHeight);
        ResizeAndRepositionButtons(MinimizedHeight);
    }

    void ResizeAndRepositionButtons(int height)
    {
        int buttonCount = Controls.Count - 1;
        int maxButtonsPerColumn = height - VerticalPadding;
        int columnCount = (int)Math.Ceiling((decimal)buttonCount / maxButtonsPerColumn);
        columnCount = columnCount < 2 ? 2 : columnCount;
        int overflowY = maxButtonsPerColumn + 2;

        int width = ColumnWidth * 2; // this needs changing to '* columnCount'
        Resize(ColumnWidth * 2, height, width, height, true);

        Point position = (HorizontalPadding / 2, 1);
        for (int i = 0; i < buttonCount; i++)
        {
            if (Controls[i] is Button button)
                button.Position = position;

            position += Direction.Down;
            if (position.Y == overflowY)
                position = (Position.X + ColumnWidth + HorizontalPadding / 2, 1);
        }

        // scroll bar setup
        _scrollBar.Position = (HorizontalPadding / 2, Height - 1);
        _scrollBar.Value = 0;
        int max = columnCount - 2;
        _scrollBar.Maximum = max < 0 ? 0 : max;
    }

    void Container_OnPageListChanged(object? sender, EventArgs args)
    {
        if (sender is not Container container) return;

        Controls.Clear();
        int buttonWidth = ColumnWidth - 2;

        foreach (Page page in container.PageList)
        {
            // create new button with a link to the page
            var button = new Button(buttonWidth, 1)
            {
                Text = page.Title,
                UseMouse = true,
                UseKeyboard = false,
            };
            button.Click += (o, e) =>
                container.CurrentPage = page;

            Controls.Add(button);
        }

        Controls.Add(_scrollBar);

        ResizeAndRepositionButtons(Height);
    }
}