using SadConsole.UI;
using SadConsole.UI.Controls;
using System.Reflection.Emit;

namespace SadExperiments.Games.PacMan;

class GameOverWindow : Window
{
    public Button RestartButton;

    public GameOverWindow() : base(40, 9)
    {
        Title = "Game Over";
        Center();

        string text = "Restart";
        RestartButton = new(text.Length + 4);
        RestartButton.Position = ((Surface.Width - RestartButton.Surface.Width) / 2, Surface.Height - 3);
        RestartButton.Text = text;
        Controls.Add(RestartButton);
    }

    public void ShowScore(int score, int level)
    {
        Print("Final score", score, "yellow", 2);
        Print("Level", level, "lightcoral", 4);
    }

    void Print(string title, int value, string color, int y)
    {
        var text = ColoredString.Parser.Parse($"{title}: [c:r f:{color}]{value}");
        Surface.Clear(1, y, Surface.Width - 2);
        Surface.Print((Surface.Width - text.Length) / 2, y, text);
    }
}