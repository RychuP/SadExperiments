using SadConsole.UI;
using SadConsole.UI.Controls;
using static SadExperiments.MainScreen.Container;

namespace SadExperiments.MainScreen;

internal class ContentsList : Page
{
    // buttons with links to pages
    readonly PageLinks _pageLinks = new();

    readonly Filter _filter = new();

    public ContentsList()
    {
        Title = "List of Contents";
        Summary = "Select a page to display.";

        // add consoles to children
        Children.Add(_pageLinks, _filter);
    }

    public void RegisterEventHandlers()
    {
        Root.PageListChanged += Container_OnPageListChanged;
        _filter.RegisterEventHandlers();
    }

    public void Container_OnPageListChanged(object? sender, EventArgs args)
    {
        _pageLinks.Controls.Clear();
        Point position = (1, 1);
        int buttonWidth = 35;

        foreach (Page page in Root.PageList)
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
                Root.CurrentPage = page;

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