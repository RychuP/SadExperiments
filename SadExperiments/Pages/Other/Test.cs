using SadConsole.UI;
using SadConsole.UI.Controls;
using SadExperiments.UI;

namespace SadExperiments.Pages;

internal class Test : Page
{
    public Test()
    {
        var console = new ControlsConsole(Width, Height);
        console.SetDefaultColors(Color.Black, Color.LightBlue);
        Children.Add(console);

        var textBox = new TextBox(10);
        console.Controls.Add(textBox);

        textBox.EditModeExit += (o, e) =>
        {
            int x = 0;
        };

        textBox.EditModeEnter += (o, e) =>
        {
            int y = 0;
        };

        textBox.EditingTextChanged += (o, e) =>
        {
            int z = 0;
        };
    }

    void OnEditingTextChanged(object? sender, EventArgs e)
    {
        if (sender is TextBox textBox)
        {
            var x = textBox.EditingText;
        }
    }
}