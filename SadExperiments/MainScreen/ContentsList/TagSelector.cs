namespace SadExperiments.MainScreen;

class TagSelector : OptionSelector
{
    Tag? _selectedTag = null;

    public TagSelector(Point position, int w, int h) : base("Filter by tag:", position, w, h)
    {
        if (Game.Instance.Screen is Container container)
            container.TagsChanged += Container_OnTagsChanged;

    }

    public Tag? SelectedTag
    {
        get => _selectedTag;
        private set
        {
            _selectedTag = value;
        }
    }

    protected virtual void Container_OnTagsChanged(object? sender, EventArgs args)
    {
        var selectedItem = (Tag?)ListBox.SelectedItem;
        ListBox.Items.Clear();
        foreach (Tag tag in Container.Instance.Tags)
            ListBox.Items.Add(tag);
        if (selectedItem.HasValue && Container.Instance.Tags.Contains(selectedItem.Value))
            ListBox.SelectedItem = selectedItem.Value;
    }

    public event EventHandler? SelectedTagChanged;
}