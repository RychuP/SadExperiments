using SadConsole.UI;

namespace SadExperiments.Pages;

internal class ContentsList : Page
{
    public ContentsList(ControlsConsole contentsList)
    {
        Title = "List of Contents";
        Summary = "Click on the page to load.";
        Children.Add(contentsList);
    }
}