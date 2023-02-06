using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadExperiments.MainScreen;

internal class ContentsList : Page
{
    // buttons with links to pages
    readonly PageLinks _pageLinks = new();

    // filters pages by tags
    readonly Filter _filter = new();

    public ContentsList()
    {
        Title = "List of Contents";
        Summary = "Select a page to display.";

        // add consoles to children
        Children.Add(_pageLinks, _filter);

        // register event handlers
        if (Game.Instance.Screen is Container container)
            container.PageListChanged += Container_OnPageListChanged;
    }

    protected virtual void Container_OnPageListChanged(object? sender, EventArgs args)
    {
        var container = sender as Container;
        if (container is null) return;

        _pageLinks.Controls.Clear();
        Point position = (1, 1);
        int buttonWidth = 35;

        foreach (Page page in container.PageList)
        {
            // create new button with a link to the page
            var button = new Button(buttonWidth, 1)
            {
                Text = page.Title,
                Position = position,
                UseMouse = true,
                UseKeyboard = false,
            };
            button.Click += (o, e) =>
                container.CurrentPage = page;

            _pageLinks.Controls.Add(button);

            // increment position
            position += Direction.Down;

            // if first column is full, start the second column
            if (position.Y == _pageLinks.Height - 1)
                position = (Program.Width - buttonWidth - 1, 1);
        }
    }

    class PageLinks : ControlsConsole
    {
        public PageLinks() : base(Program.Width, Program.Height - Filter.MinimizedHeight)
        {
            Position = (0, Filter.MinimizedHeight);
        }
    }
}