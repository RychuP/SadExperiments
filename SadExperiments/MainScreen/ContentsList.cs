namespace SadExperiments.MainScreen;

internal class ContentsList : Page
{
    // buttons with links to pages
    public PageLinks PageLinks { get; init; } = new();

    // filters pages by tags
    public Filter Filter { get; init; } = new();

    public ContentsList()
    {
        Title = "List of Contents";
        Summary = "Select a page to display.";

        // add consoles to children
        Children.Add(PageLinks, Filter);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Container)
            Filter.DisableKeyboardInTextBoxes();
        base.OnParentChanged(oldParent, newParent);
    }
}