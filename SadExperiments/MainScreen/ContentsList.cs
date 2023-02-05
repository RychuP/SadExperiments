using SadConsole.UI;
using SadConsole.UI.Controls;
using static SadExperiments.MainScreen.Container;

namespace SadExperiments.MainScreen;

internal class ContentsList : Page
{
    readonly Filter _filter;

    // buttons with links to pages
    readonly ControlsConsole _buttons;

    public ContentsList()
    {
        Title = "List of Contents";
        Summary = "Select a page to display.";

        // create filters console
        _filter = new Filter()
        {
            Parent = this,
        };

        // create page buttons
        _buttons = new ControlsConsole(Program.Width, Program.Height - Filter.MinimizedHeight)
        {
            Parent = this,
            Position = (0, Filter.MinimizedHeight),
        };
    }

    public void Container_OnPageListChanged(object? sender, EventArgs args)
    {
        _buttons.Controls.Clear();
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

            _buttons.Controls.Add(button);

            // increment position
            position += Direction.Down;

            // if first column is full, start the second column
            if (position.Y == _buttons.Height - 1)
                position = (Program.Width - buttonWidth - 1, 1);
        }
    }
}