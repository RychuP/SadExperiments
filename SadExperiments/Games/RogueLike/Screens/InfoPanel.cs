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
    readonly string[] _welcomeMessages =
    {
        "Welcome to the dungeon!",
        "Orcs, trolls and treasure await.",
        "Arrow keys to move, space to wait."
    };

    public InfoPanel(Dungeon dungeon) : 
        base(Program.Width - StatusPanel.Width - InventoryPanel.Width, StatusPanel.Height)
    {
        Position = (StatusPanel.Width, Program.Height - StatusPanel.Height);
        dungeon.MapGenerated += Dungeon_OnMapGenerated;
        dungeon.Player.Moved += Player_OnMoved;
        dungeon.Player.Attacked += Actor_OnAttacked;
        dungeon.Player.Died += Actor_OnDied;
    }

    public void Reset()
    {
        _currentLine = -1;
        _playerMoveCount = 0;
        Surface.Clear();
    }

    void Print(string text)
    {
        if (_currentLine == Surface.Height - 1)
            Surface.ShiftUp();
        else 
            _currentLine++;

        // random message color
        Color color;
        do color = Program.RandomColor;
        while (color.GetBrightness() < 0.8f);

        // print message
        Surface.Print(Padding, _currentLine, text.Align(HorizontalAlignment.Left, Surface.Width), color);
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

        if (o is Player)
            Print("Press Enter to try again.");
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
        foreach (var text in _welcomeMessages)
            Print(text);

        // attach event handlers to enemies
        foreach (var positionPair in e.Actors)
        {
            if (positionPair.Item is not Enemy enemy) continue;
            enemy.Attacked += Actor_OnAttacked;
            enemy.Died += Actor_OnDied;
        }
    }
}