using SadConsole;
using SadExperiments.Games.RogueLike.World.Entities;
using System.Diagnostics;

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
        var query = inventory.Items.Cast<Entity>().GroupBy(i => i.Name).ToArray();
        for (int k = 0; k < Surface.Height; k++)
        {
            if (k < query.Length)
            {
                var group = query[k];
                Entity first = group.First();
                Print(k, $"{(char)first.Glyph} x {group.Count()}");
                //Print(k, $"{(char)first.Glyph}", first.Color);
            }
            else
                Surface.Clear(Padding, k, Surface.Width);
        }

        //for (int i = 0; i < Surface.Height; i++)
        //{
        //    if (i < inventory.Items.Count)
        //        Surface.Print(Padding, i, inventory.Items[i].ToString());
        //    else
        //        Surface.Clear(Padding, i, Surface.Width);
        //}
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