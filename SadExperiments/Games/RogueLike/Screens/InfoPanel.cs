using SadExperiments.Games.RogueLike.World;
using SadExperiments.Games.RogueLike.World.Entities;
using GoRogue.Random;

namespace SadExperiments.Games.RogueLike.Screens;

// displays messages
internal class InfoPanel : PanelWithSeparator
{
    const int Padding = 2;
    int _currentLine = -1;
    int _playerMoveCount = 0;
    readonly string[] _deathDescriptions =
    {
        "receives a mortal wound",
        "falls to the ground",
        "looses all hp points",
        "gives in to the heavy blow"
    };
    

    public InfoPanel(Dungeon dungeon) : 
        base(Program.Width - StatusPanel.Width - InventoryPanel.Width, StatusPanel.Height)
    {
        Position = (StatusPanel.Width, Program.Height - StatusPanel.Height);
        dungeon.MapGenerated += Dungeon_OnMapGenerated;
        dungeon.Player.Moved += Player_OnMoved;
    }

    public void Reset()
    {
        _currentLine = -1;
        _playerMoveCount = 0;
        Surface.Clear();
    }

    void Print(string text)
    {
        // clear the welcome message
        if (_currentLine == -1)
            Surface.Clear();

        // make space for the new line
        if (_currentLine == Surface.Height - 1)
            Surface.ShiftUp();

        // advance pointer
        else 
            _currentLine++;

        // print message
        Surface.Print(Padding, _currentLine, text.Align(HorizontalAlignment.Left, Surface.Width));
    }

    void Actor_OnAttacked(object? o, CombatEventArgs e)
    {
        if (o is not Actor actor) return;
        if (e.Damage > 0)
            Print($"{actor} attacks {e.Target} for {e.Damage} damage.");
        else
            Print($"{actor} attacks {e.Target} but does no damage.");
    }

    void Actor_OnDied(object? o, EventArgs e)
    {
        if (o is not Actor actor) return;

        int i = GlobalRandom.DefaultRNG.NextInt(_deathDescriptions.Length);
        string description = _deathDescriptions[i];
        Print($"The {actor} {description} and dies!");
    }

    // shifts up messages after a few player moves
    void Player_OnMoved(object? o, EventArgs e)
    {
        if (++_playerMoveCount >= 5)
        {
            _playerMoveCount = 0;
            Surface.ShiftUp();

            // shift the pointer for the print method as well
            if (_currentLine >= 0)
                _currentLine--;
        }
    }

    void Dungeon_OnMapGenerated(object? o, MapGeneratedEventArgs e)
    {
        // print welcome message
        Surface.Print(Padding, 0, "Welcome to the dungeon!");
        Surface.Print(Padding, 1, "Orcs, trolls and treasure await.");
        Surface.Print(Padding, 2, "Arrow keys to move, space to wait.");

        // attach event handlers to actors
        foreach (var positionPair in e.Actors)
        {
            if (positionPair.Item is not Actor actor) continue;
            actor.Attacked += Actor_OnAttacked;
            actor.Died += Actor_OnDied;
        }
    }
}