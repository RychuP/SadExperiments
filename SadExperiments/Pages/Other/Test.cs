using SadConsole.UI;
using SadConsole.UI.Controls;
using SadExperiments.UI;

namespace SadExperiments.Pages;

internal class Test : Page
{
    public Test()
    {
        var console = new ControlsConsole(Width, Height);
        Children.Add(console);

        string text = "Sample Button";
        var button = new Button(text.Length + 4, 1)
        {
            Text = text,
            Position = (1, 1),
        };
        console.Controls.Add(button);

        var dotNav = new DotNavigation(5);
        console.Children.Add(dotNav);
        dotNav.Position = (20, 20);

        button.Click += (o, e) =>
        {
            button.Position = (10, 10);
            dotNav.Position = (10, 11);
        };
    }
}