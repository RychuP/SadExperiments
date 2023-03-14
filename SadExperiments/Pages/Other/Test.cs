namespace SadExperiments.Pages;

class Test : Page
{
    public Test()
    {
        var child = new Child();
        Children.Add(child);
        Children.Remove(child);
        Children.Add(child);
    }
}

class Child : ScreenSurface
{
    public Child() : base(1, 1) { }
    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        base.OnParentChanged(oldParent, newParent);
    }
}