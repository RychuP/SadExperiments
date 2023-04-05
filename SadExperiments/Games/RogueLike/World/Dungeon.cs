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
        Player.Died += Actor_OnDied;

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
    public bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Keys.Left) || keyboard.IsKeyPressed(Keys.H))
        {
            PlayerMoveOrAttack(Direction.Left);
            return true;
        }
        else if (keyboard.IsKeyPressed(Keys.Right) || keyboard.IsKeyPressed(Keys.L))
        {
            PlayerMoveOrAttack(Direction.Right);
            return true;
        }
        else if (keyboard.IsKeyPressed(Keys.Up) || keyboard.IsKeyPressed(Keys.K))
        {
            PlayerMoveOrAttack(Direction.Up);
            return true;
        }
        else if (keyboard.IsKeyPressed(Keys.Down) || keyboard.IsKeyPressed(Keys.J))
        {
            PlayerMoveOrAttack(Direction.Down);
            return true;
        }
        else if (keyboard.IsKeyPressed(Keys.Space))
        {
            PlayerWait();
            return true;
        }
        else if (keyboard.IsKeyPressed(Keys.D))
        {
            var item = GetEntityAt<Item>(Player.Position);
            if (item is null)
                OnFailedAction("There it nothing to pick up here.");
            else if (item is ICarryable carryable)
            {
                if (Player.TryCollect(carryable))
                    RemoveEntity(item);
            }
            else
                OnFailedAction("This type of an item cannot be picked up.");
            return true;
        }
        else if (keyboard.IsKeyPressed(Keys.A))
        {
            Player.TryConsumeHealthPotion();
            return true;
        }

        return false;
    }

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
            RemoveEntity(posPair.Item);
    }

    void SpawnPlayer(Point position)
    {
        Player.Position = position;
        AddEntity(Player);
    }

    void SpawnEnemies(IReadOnlyList<Rectangle> rooms)
    {
        foreach (var room in rooms)
        {
            int enemies = GlobalRandom.DefaultRNG.NextInt(0, MaxMonstersPerRoom + 1);
            for (int i = 0; i < enemies; i++)
            {
                Enemy enemy = GlobalRandom.DefaultRNG.PercentageCheck(80f) ? new Orc() : new Troll();
                enemy.Position = GlobalRandom.DefaultRNG.RandomPosition(room, pos => WalkabilityView[pos]);
                enemy.Died += Actor_OnDied;
                AddEntity(enemy);
            }
        }
    }

    void SpawnHealthPotions(IReadOnlyList<Rectangle> rooms)
    {
        // Generate between zero and the max potions per room.
        foreach (var room in rooms)
        {
            int amount = GlobalRandom.DefaultRNG.NextInt(0, MaxPotionsPerRoom + 1);
            for (int i = 0; i < amount; i++)
            {
                var position = GlobalRandom.DefaultRNG.RandomPosition(room, pos => WalkabilityView[pos]);
                var potion = new HealthPotion { Position = position };
                AddEntity(potion);
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
        // get enemies in view
        var enemies = Entities.GetLayer((int)EntityLayer.Actors).Items
            .Where(o => o is Enemy e && PlayerFOV.CurrentFOV.Contains(e.Position))
            .Cast<Enemy>();

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

    void Actor_OnDied(object? o, EventArgs e)
    {
        if (o is not Actor actor) return;
        RemoveEntity(actor);
        AddEntity(new Corpse(actor));
    }

    void OnFailedAction(string message)
    {
        var args = new FailedActionEventArgs(message);
        FailedAction?.Invoke(this, args);
    }

    void OnMapGenerated(IReadOnlyList<Rectangle> rooms)
    {
        SpawnPlayer(rooms[0].Center);
        SpawnEnemies(rooms);
        SpawnHealthPotions(rooms);

        var actors = Entities.GetLayer((int)EntityLayer.Actors);
        var args = new MapGeneratedEventArgs(actors);
        MapGenerated?.Invoke(this, args);
    }
    #endregion

    #region Events
    public event EventHandler<MapGeneratedEventArgs>? MapGenerated;
    public event EventHandler<FailedActionEventArgs>? FailedAction;
    public event EventHandler? FOVChanged;
    #endregion
}