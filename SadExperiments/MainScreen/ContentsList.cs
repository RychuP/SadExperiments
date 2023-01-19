using SadConsole.UI;

namespace SadExperiments.MainScreen;

internal class ContentsList : Page
{
    public ContentsList(ControlsConsole contentsList)
    {
        Title = "List of Contents";
        Summary = "Click on the page to load.";
        Children.Add(contentsList);
        IsVisible = false;
    }

    public bool IsBeingShown => IsFocused;
}