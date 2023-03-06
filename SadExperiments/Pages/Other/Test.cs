namespace SadExperiments.Pages;

class Test : Page
{
    public Test()
    {
        double i = 2.7d;
        double x = i % 1;
        Surface.Print(0, 0, $"{x}");
        int y = Convert.ToInt32(i - x);
        Surface.Print(0, 1, y.ToString() );
    }
}