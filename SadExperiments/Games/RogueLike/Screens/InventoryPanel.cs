using SadExperiments.Games.RogueLike.World.Entities;

namespace SadExperiments.Games.RogueLike.Screens;

// displays player inventory
internal class InventoryPanel : PanelWithSeparator
{
    public const int Width = 15;

    public InventoryPanel(Player player) : base(StatusPanel.Width, StatusPanel.Height)
    {
        Position = (Program.Width - Surface.Width, Program.Height - Surface.Height);
        player.Collected += Player_OnCollected;
        player.Consumed += Player_OnConsumed;
    }

    public void Reset() =>
        Surface.Clear();

    void Refresh(Inventory inventory)
    {
        for (int i = 0; i < Surface.Height; i++)
        {
            if (i < inventory.Items.Count)
                Surface.Print(Padding, i, inventory.Items[i].ToString());
            else
                Surface.Clear(Padding, i, Surface.Width);
        }
    }

    void Player_OnCollected(object? o, EventArgs e)
    {
        if (o is not Player player) return;
        Refresh(player.Inventory);
    }

    void Player_OnConsumed(object? o, EventArgs e)
    {
        if (o is not Player player) return;
        Refresh(player.Inventory);
    }
}