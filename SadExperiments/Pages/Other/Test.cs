namespace SadExperiments.Pages;

class Test : Page
{
    public Test()
    {
        int i = 4 << -1;
        Surface.Print(0, 0, i.ToString());
    }
}