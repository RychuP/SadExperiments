namespace SadExperiments.Games.RogueLike.Screens;

// displays player inventory
internal class InventoryPanel : PanelWithSeparator
{
    public const int Width = 15;

    public InventoryPanel() : base(StatusPanel.Width, StatusPanel.Height)
    {
        Position = (Program.Width - Surface.Width, Program.Height - Surface.Height);

        Print(0, "Heal potion");
    }

    void Print(int row, string text)
    {
        Surface.Print(2, row, text.Align(HorizontalAlignment.Left, Surface.Width));
    }
}