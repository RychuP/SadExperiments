namespace SadExperiments.MainScreen;

internal class ContentsList : Page
{
    // buttons with links to pages
    readonly PageLinks _pageLinks;

    // filters pages by tags
    readonly Filter _filter = new();

    public ContentsList()
    {
        Title = "List of Contents";
        Summary = "Select a page to display.";

        _pageLinks = new(_filter);

        // add consoles to children
        Children.Add(_pageLinks, _filter);
    }
}