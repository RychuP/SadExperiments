using SadConsole.UI.Controls;
using SadConsole.UI;

namespace SadExperiments.Games.Tetris;

class StartWindow : Window
{
    public Button StartButton;

    public StartWindow() : base(40, 9)
    {
        Title = "Tetris";
        Center();

        string text = "Controls:";
        Surface.Print((Surface.Width - text.Length) / 2, 2, text);
        text = "x, z, left, right, down, space";
        Surface.Print((Surface.Width - text.Length) / 2, 4, text);

        text = "Start Game";
        StartButton = new(text.Length + 4);
        StartButton.Position = ((Surface.Width - StartButton.Surface.Width) / 2, Surface.Height - 3);
        StartButton.Text = text;
        Controls.Add(StartButton);
    }
}