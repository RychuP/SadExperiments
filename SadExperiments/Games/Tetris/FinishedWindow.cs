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
        RestartButton.Position = (Surface.Width / 2 - RestartButton.Surface.Width / 2, Surface.Height - 3);
        RestartButton.Text = text;
        Controls.Add(RestartButton);
    }

    public void ShowFinals(int score, int level)
    {
        string text = $"Final score is: {score}";
        Surface.Clear(1, 2, Surface.Width - 2);
        Surface.Print(Surface.Width / 2 - text.Length / 2, 2, text);
        text = $"Level reached: {level}";
        Surface.Clear(1, 4, Surface.Width - 2);
        Surface.Print(Surface.Width / 2 - text.Length / 2, 4, text);
    }
}