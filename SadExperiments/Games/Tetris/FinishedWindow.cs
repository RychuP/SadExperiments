using SadConsole.UI.Controls;
using SadConsole.UI;

namespace SadExperiments.Games.Tetris;

class FinishedWindow : Window
{
    public Button RestartButton;

    public FinishedWindow() : base(40, 9)
    {
        Title = "Game Over";
        Center();

        string text = "Restart";
        RestartButton = new(text.Length + 4);
        RestartButton.Position = ((Surface.Width - RestartButton.Surface.Width) / 2, Surface.Height - 3);
        RestartButton.Text = text;
        Controls.Add(RestartButton);
    }

    public void ShowFinals(int score, int level, int lines)
    {
        var text = ColoredString.Parser.Parse($"Final score: [c:r f:yellow]{score}");
        Surface.Clear(1, 2, Surface.Width - 2);
        Surface.Print((Surface.Width - text.Length) / 2, 2, text);
        text = ColoredString.Parser.Parse($"Level: [c:r f:lightcoral]{level}[c:undo], Lines: [c:r f:cyan]{lines}");
        Surface.Clear(1, 4, Surface.Width - 2);
        Surface.Print((Surface.Width - text.Length) / 2, 4, text);
    }
}