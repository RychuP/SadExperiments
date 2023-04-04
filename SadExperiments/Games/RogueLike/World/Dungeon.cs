using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.Random;
using SadExperiments.Games.RogueLike.Screens;
using SadExperiments.Games.RogueLike.World.Entities;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace SadExperiments.Games.RogueLike.World;

internal class Dungeon : Map
{
    #region Fields
    // map generation
    const int MinRooms = 20;
    const int MaxRooms = 30;
    const int RoomMinSize = 8;
    const int RoomMaxSize = 15;
    const int MaxMonstersPerRoom = 2;
    const int MaxPotionsPerRoom = 2;
    readonly IEnumerable<GenerationStep> _genAlgorithm;

    // view
    Rectangle _view = new(0, 0, Renderer.Width, Renderer.Height);
    #endregion

    #region Constructors
    public Dungeon() : base(Renderer.Width * 2, Renderer.Height * 2, 2, Distance.Chebyshev)
    {
        // generation algorithm
        _genAlgorithm = DefaultAlgorithms.DungeonMazeMapSteps(null, MinRooms, MaxRooms, 
            RoomMinSize, RoomMaxSize, saveDeadEndChance: 0);

        // player
        Player.Moved += Player_OnMoved;

        // fov
        PlayerFOV.Recalculated += FOV_OnRecalculated;
    }
    #endregion

    #region Properties
    public Player Player { get; } = new();

    public Rectangle View
    {
        get => _view;
        private set
        {
            if (_view == value) return;
            if (!Terrain.Bounds().Contains(value))
                throw new InvalidOperationException("View is out of bounds.");
            _view = value;
        }
    }

    public IEnumerable<Enemy> VisibleEnemies => Entities
        .GetLayer((int)EntityLayer.Actors)
        .Where(o => o.Item is Enemy && PlayerFOV.CurrentFOV.Contains(o.Position))
        .Select(o => o.Item)
        .Cast<Enemy>();
    #endregion

    #region Methods
    public void Reset()
    {
        Player.Reset();
        RemoveAllEntities();
        PlayerExplored.Clear();
        Generate();
    }

    public void PlayerMoveOrAttack(Direction direction)
    {
        if (ActorMoveOrAttack(Player, direction))
            MoveEnemies();
    }

    public void PlayerWait() =>
        MoveEnemies();

    void Generate()
    {
        var generator = new Generator(Width, Height).ConfigAndGenerateSafe(g => g.AddSteps(_genAlgorithm));
        var gridView = generator.Context.GetFirst<ISettableGridView<bool>>("WallFloor");
        var rooms = generator.Context.GetFirst<ItemList<Rectangle>>("Rooms");
        ApplyTerrainOverlay(gridView, (pos, val) => val ? new Floor(pos) : new Wall(pos));
        OnMapGenerated(rooms.Items);
    }

    void CenterViewOnPlayer()
    {
        var view = View.WithCenter(Player.Position);
        int x = view.Position.X < 0 ? 0 : view.Position.X;
        int y = view.Position.Y < 0 ? 0 : view.Position.Y;
        x = view.MaxExtentX >= Width ? Width - view.Width : x;
        y = view.MaxExtentY >= Height ? Height - view.Height : y;
        View = view.WithPosition(new Point(x, y));
    }

    void RemoveAllEntities()
    {
        foreach (var posPair in Entities)
        {
            if (posPair.Item is Enemy enemy)
                enemy.Died -= Enemy_OnDied;
            RemoveEntity(posPair.Item);
        }
    }

    void SpawnPlayer(Point position)
    {
        Player.Position = position;
        AddEntity(Player);
    }

    void SpawnMonsters(IReadOnlyList<Rectangle> rooms)
    {
        foreach (var room in rooms)
        {
            int enemies = GlobalRandom.DefaultRNG.NextInt(0, MaxMonstersPerRoom + 1);
            for (int i = 0; i < enemies; i++)
            {
                Enemy enemy = GlobalRandom.DefaultRNG.PercentageCheck(80f) ? new Orc() : new Troll();
                enemy.Position = GlobalRandom.DefaultRNG.RandomPosition(room, pos => WalkabilityView[pos]);
                enemy.Died += Enemy_OnDied;
                AddEntity(enemy);
            }
        }
    }

    // returns true if the action was successful
    bool ActorMoveOrAttack(Actor actor, Direction direction)
    {
        Point newPosition = actor.Position + direction;
        Actor? other = GetEntityAt<Actor>(newPosition);
        if (GameObjectCanMove(actor, newPosition))
        {
            actor.Position = newPosition;
            return true;
        }
        else if (other is not null && (actor is Player || other is Player))
        {
            actor.Attack(other);
            return true;
        }
        return false;
    }

    void MoveEnemies()
    {
        var enemies = Entities.GetLayer((int)EntityLayer.Actors).Items
            .Where(o => o is Enemy e && PlayerFOV.CurrentFOV.Contains(e.Position))
            .Select(o => o as Enemy);
        foreach (var enemy in enemies)
        {
            if (enemy is null) continue;
            var path = AStar.ShortestPath(enemy.Position, Player.Position);
            if (path == null) continue;
            var firstPoint = path.GetStep(0);
            ActorMoveOrAttack(enemy, Direction.GetDirection(enemy.Position, firstPoint));
        }
    }
    #endregion

    #region Event Handlers
    void Player_OnMoved(object? o, GameObjectPropertyChanged<Point> e)
    {
        PlayerFOV.Calculate(e.NewValue, Player.FOVRadius, DistanceMeasurement);
        CenterViewOnPlayer();
    }

    void FOV_OnRecalculated(object? o, EventArgs e)
    {
        FOVChanged?.Invoke(this, e);
    }

    void Enemy_OnDied(object? o, EventArgs e)
    {
        if (o is not Enemy enemy) return;
        enemy.Died -= Enemy_OnDied;
        RemoveEntity(enemy);
        AddEntity(new Corpse(enemy));
    }

    void OnMapGenerated(IReadOnlyList<Rectangle> rooms)
    {
        SpawnPlayer(rooms[0].Center);
        SpawnMonsters(rooms);

        var actors = Entities.GetLayer((int)EntityLayer.Actors);
        var args = new MapGeneratedEventArgs(actors);
        MapGenerated?.Invoke(this, args);
    }
    #endregion

    #region Events
    public event EventHandler<MapGeneratedEventArgs>? MapGenerated;
    public event EventHandler? FOVChanged;
    #endregion
}