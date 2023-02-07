using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadExperiments.Pages;

internal class Crash : Page
{
    public Crash()
    {
        var console = new ControlsConsole(Width, Height);
        var button = new Button(10) { Position = (20, 20) };
        console.Controls.Add(button);
        Children.Add(console);
        console.Resize(Width, Height, Width * 2, Height, true);
    }
}