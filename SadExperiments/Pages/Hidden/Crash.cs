using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadExperiments.Pages;

internal class Crash : Page
{
    public Crash()
    {
        Title = "Engine Crash";
        var console = new ControlsConsole(80, 30);
        var button = new Button(10) { Position = (20, 20) };
        console.Controls.Add(button);
        Children.Add(console);
        console.Resize(80, 30, 160, 30, true);
    }
}