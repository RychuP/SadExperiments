using SadConsole.UI;

namespace SadExperiments.MainScreen;

internal class ContentsList : Page
{
    readonly Filter _filter;

    public ContentsList(ControlsConsole contentsList)
    {
        Title = "List of Contents";
        Summary = "Select a page to display.";
        Children.Add(contentsList);
        IsVisible = false;

        // TODO: replace contentsList with this
        //var buttons = new SubPage(Width, Height - 5)
        //{
        //    Parent = this,
        //    Position = new Point(0, 5),
        //};
        //SubPage.Surface.SetDefaultColors(Color.White, Color.Black);
        //Surface.SetDefaultColors(Color.White, Color.Black);

        // create filters console
        _filter = new Filter()
        {
            Parent = this,
        };
    }

    public void Show()
    {
        IsVisible = true;
        IsFocused = true;
    }

    public void Hide()
    {
        IsVisible = false;
        _filter.Minimize();
    }
}