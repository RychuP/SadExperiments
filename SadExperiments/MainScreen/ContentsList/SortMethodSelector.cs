namespace SadExperiments.MainScreen;

class SortMethodSelector : OptionSelector
{
    public SortMethodSelector(Point position, int w, int h) : base("Sort method:", position, w, h)
    {
        ListBox.Items.Add(SortMethod.Alphabetical);
        ListBox.Items.Add(SortMethod.Date);
        ListBox.Items.Add(SortMethod.Default);

        ClearButton.Click += (o, e) => ListBox.Execute(SortMethod.Default);
    }

    
}

enum SortMethod
{
    Default,
    Alphabetical,
    Date,
}