namespace SadExperiments.MainScreen;

class SortMethodSelector : OptionSelector
{
    public SortMethodSelector(Point position, int w, int h) : base("Sort method:", position, w, h)
    {
        ListBox.Items.Add(SortMethods.Alphabetical);
        ListBox.Items.Add(SortMethods.Date);
    }
}

enum SortMethods
{
    Alphabetical,
    Date,
}