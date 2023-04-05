using GoRogue.GameFramework;
using GoRogue.SpatialMaps;
using SadExperiments.Games.RogueLike.World;
using SadExperiments.Games.RogueLike.World.Entities;

namespace SadExperiments.Games.RogueLike.Screens;

// displays health bars
internal class StatusPanel : ScreenSurface
{
    readonly List<Enemy> _trackedEnemies = new(10);
    public const int Width = 15;
    public const int Height = 4;

    public StatusPanel(Dungeon dungeon) : base(Width, Height)
    {
        Surface.SetDefaultColors(Colors.BottomPanelFG, Colors.BottomPanelBG);
        Position = (0, Program.Height - Height);

        dungeon.Player.HPChanged += Player_OnHPChanged;
        dungeon.FOVChanged += Dungeon_OnFOVChanged;
        dungeon.MapGenerated += Dungeon_OnMapGenerated;
        dungeon.ObjectRemoved += Dungeon_OnObjectRemoved;
    }

    public void Reset()
    {
        Surface.Clear();
        _trackedEnemies.Clear();
    }

    static string EnemyHPText(Enemy enemy)
    {
        //string id = enemy.ID.ToString();
        //id = id.Substring(id.Length - 3);
        string name = GetAlignedName($"{enemy}");
        return $"{name} HP: {enemy.HP}";
    }

    static string GetAlignedName(string name) =>
        name.Align(HorizontalAlignment.Left, 6);

    void Print(int row, string text)
    {
        Surface.Print(1, row, text.Align(HorizontalAlignment.Left, Surface.Width));
    }

    void PrintPlayerHP(Player player)
    {
        string name = GetAlignedName("Player");
        Print(0, $"{name} HP: {player.HP}");
    }

    void PrintEnemyHP()
    {
        for (int i = 0; i < Surface.Height - 1; i++)
        {
            if (i < _trackedEnemies.Count)
            {
                var enemy = _trackedEnemies[i];
                Print(i + 1, EnemyHPText(enemy));
            }
            else
                Surface.Clear(0, i + 1, Surface.Width);
        }
    }

    void Player_OnHPChanged(object? o, EventArgs e)
    {
        if (o is not Player player) return;
        PrintPlayerHP(player);
    }

    void Enemy_OnHPChanged(object? o, EventArgs e)
    {
        if (o is not Enemy enemy) return;
        if (_trackedEnemies.Contains(enemy))
        {
            // move the enemy currently being battled to the top
            if (_trackedEnemies.IndexOf(enemy) != 0)
            {
                _trackedEnemies.Remove(enemy);
                _trackedEnemies.Insert(0, enemy);
            }
            PrintEnemyHP();
        }
    }

    void Dungeon_OnObjectRemoved(object? o, ItemEventArgs<IGameObject> e)
    {
        if (e.Item is not Enemy enemy) return;
        if (_trackedEnemies.Contains(enemy))
        {
            _trackedEnemies.Remove(enemy);
            PrintEnemyHP();
        }
    }

    void Dungeon_OnMapGenerated(object? o, MapGeneratedEventArgs e)
    {
        if (o is not Dungeon dungeon) return;
        PrintPlayerHP(dungeon.Player);

        // track visible enemies
        foreach (var enemy in dungeon.VisibleEnemies)
            _trackedEnemies.Add(enemy);
        PrintEnemyHP();

        // add hp changed handlers
        foreach (var posPair in e.Actors)
            if (posPair.Item is Enemy enemy)
                enemy.HPChanged += Enemy_OnHPChanged;
    }

    void Dungeon_OnFOVChanged(object? o, EventArgs e)
    {
        if (o is not Dungeon dungeon) return;

        var enemiesToBeRemoved = new List<Enemy>();

        // mark tracked enemies that went out of view for removal
        foreach (var trackedEnemy in _trackedEnemies)
        {
            if (!dungeon.VisibleEnemies.Contains(trackedEnemy))
                enemiesToBeRemoved.Add(trackedEnemy);
        }

        // remove enemies 
        foreach (var enemyForRemoval in enemiesToBeRemoved)
            _trackedEnemies.Remove(enemyForRemoval);
        
        // add missing visible enemies 
        foreach (var enemy in dungeon.VisibleEnemies)
        {
            if (!_trackedEnemies.Contains(enemy))
                _trackedEnemies.Add(enemy);
        }

        // redraw
        PrintEnemyHP();
    }
}