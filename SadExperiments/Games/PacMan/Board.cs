using GoRogue.Pathing;
using GoRogue.Random;
using SadConsole.Effects;
using SadConsole.Entities;
using SadExperiments.Games.PacMan.Ghosts;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;
using static System.Net.Mime.MediaTypeNames;

namespace SadExperiments.Games.PacMan;

// Pac-Dot = 10 Pts
// Power Pellet = 50 Pts
// 1st Ghost = 200 Pts
// 2nd Ghost = 400 Pts
// 3rd Ghost = 800 Pts
// 4th Ghost = 1600 Pts
// Cherry = 100 Pts
// Strawberry = 300 Pts
// Orange = 500 Pts
// Apple = 700 Pts
// Melon = 1000 Pts
// Galaxian = 2000 Pts
// Bell = 3000 Pts
// Key = 5000 Pts
class Board : ScreenSurface
{
    #region Fields
    readonly AdjacencyRule _adjacencyRule = AdjacencyRule.EightWay;
    readonly Renderer _renderer = new();
    readonly AStar _aStar;
    bool _isPaused = true;

    // allows slowing down pacman each time they eat a dot
    bool _dotEatenThisFrame = false;

    // custom gameplay feature
    readonly bool ReplenishDotsOnLiveLost = false;
    readonly List<Dot> _removedDots;

    // level complete animation
    readonly TimeSpan _colorChangeDuration = TimeSpan.FromSeconds(0.33d);
    bool _playingLevelCompleteAnimation = false;
    TimeSpan _animationTimeDelta = TimeSpan.Zero;
    const int WallColorChangeMax = 6;
    int _wallColorChangeCount = 0;

    // debug screen
    readonly ScreenSurface _debug;
    TimeSpan _secondCounter = TimeSpan.Zero;
    #endregion Fields

    #region Constructors
    public Board(Level level, Game game) : base(level.Width, level.Height, level.Tiles)
    {
        // debug surface
        _debug = new(Program.Width, 1)
        {
            //Parent = this,
            Position = (-3, -1)
        };

        // setup
        IsFocused = true;
        UsePixelPositioning = true;
        Font = Fonts.Maze;
        FontSize = Game.DefaultFontSize;
        Parent = Game = game;

        // astar
        var walkabilityView = new WalkabilityView(this);
        _aStar = new AStar(walkabilityView, Distance.Manhattan);

        // walls
        DrawWalls(level);

        // dots
        _removedDots = new List<Dot>(level.Dots.Count);
        DrawDots(level);

        // ghost house
        var spawner = level.Tiles.Where(t => t is Spawner).FirstOrDefault();
        if (spawner != null)
            GhostHouse = new(this, spawner.Position);
        else
            throw new ArgumentException("Tiles do not contain a spawner location.");
        GhostHouse.PowerDotStarted += GhostHouse_OnPowerDotStarted;
        GhostHouse.PowerDotDepleted += GhostHouse_OnPowerDotDepleted;
        GhostHouse.ModeChanged += GhostHouse_OnModeChanged;
        Children.Add(GhostHouse);

        // player
        Player = new Player(this, level.Start);
        Player.DeathAnimationFinished += Player_OnDeathAnimationFinished;

        // small pause at the beginning
        SadComponents.Add(new StartPause());
    }
    #endregion Constructors

    #region Properties
    public Game Game { get; init; }
    public Player Player { get; init; }
    public GhostHouse GhostHouse { get; init; }
    public bool IsPaused
    {
        get => _isPaused;
        set
        {
            _isPaused = value;
            if (_isPaused)
                GhostHouse.PauseRunningTimers();
            else
                GhostHouse.UnpausePrevRunningTimers();
        }
    }
    #endregion Properties

    #region Methods
    // checks if the position is valid and walkable
    public bool IsWalkable(Point position) =>
        Surface.Area.Contains(position) && Surface[position.ToIndex(Surface.Width)] is Floor;

    // checks if the position is reachable from the player start point
    public bool IsReachable(Point position) =>
        _aStar.ShortestPath(Player.Start, position) is not null;

    // checks if the position is a portal and returns a matching portal destination if found
    public bool IsPortal(Point position, out Portal? matchingPortalDestination)
    {
        if (Surface.Area.Contains(position))
        {
            int index = position.ToIndex(Surface.Width);
            if (Surface[index] is Portal portal)
            {
                matchingPortalDestination = Surface.Where(cg => cg is Portal p
                    && p.Position != position && p.Id == portal.Id).FirstOrDefault() as Portal;
                if (matchingPortalDestination is null)
                    throw new ArgumentException($"Map doesn't contain a destination portal for id: {portal.Id}");
                return true;
            }
        }

        matchingPortalDestination = null;
        return false;
    }

    bool GamePlayIsOn() =>
        Children.Contains(Player) && !IsPaused && !_playingLevelCompleteAnimation && !Player.IsDead;

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        // TODO: remove or change this when debug finished
        //if (keyboard.HasKeysPressed && keyboard.IsKeyPressed(Keys.P))
        //{
        //    TogglePause();
        //}
        //else 
        if (GamePlayIsOn() && keyboard.HasKeysDown)
        {
            if (keyboard.IsKeyDown(Keys.Right))
            {
                Player.NextDirection = Direction.Right;
                return true;
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                Player.NextDirection = Direction.Left;
                return true;
            }
            else if (keyboard.IsKeyDown(Keys.Up))
            {
                Player.NextDirection = Direction.Up;
                return true;
            }
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                Player.NextDirection = Direction.Down;
                return true;
            }
        }
        return base.ProcessKeyboard(keyboard);
    }

    public override void Update(TimeSpan delta)
    {
        if (GamePlayIsOn())
        {
            UpdateSpritePositions(delta);
            
            if (!_playingLevelCompleteAnimation)
                CheckForSpriteCollisions();
        }

        else if (Player.IsDead)
            Player.PlayDeathAnimation(delta);

        else if (_playingLevelCompleteAnimation)
            PlayLevelCompleteAnimation(delta);
        
        // debug info
        if (!IsPaused)
        {
            _secondCounter += delta;
            string text = $"    Mode Changes: {GhostHouse.ModeChangesCount}, Seconds: {_secondCounter.Seconds:000}";
            int x = _debug.Surface.Width - text.Length - 3;
            _debug.Surface.Print(x, 0, text);
        }

        base.Update(delta);
    }

    void PlayLevelCompleteAnimation(TimeSpan delta)
    {
        _animationTimeDelta += delta;
        if (_animationTimeDelta >= _colorChangeDuration)
        {
            if (++_wallColorChangeCount <= WallColorChangeMax)
            {
                var color = _wallColorChangeCount % 2 == 0 ? Appearances.Wall.Foreground : Appearances.WallFlash;
                _animationTimeDelta = TimeSpan.Zero;
                ChangeMazeColor(color);
            }
            else
            {
                _playingLevelCompleteAnimation = false;
                OnLevelCompleteAnimationFinished();
            }
        }
    }

    void UpdateSpritePositions(TimeSpan delta)
    {
        foreach (var child in Children)
        {
            if (child is Sprite sprite)
            {
                sprite.UpdateAnimation(delta);

                if (sprite is Ghost)
                    sprite.UpdatePosition();

                else if (sprite is Player player)
                {
                    // don't update the player's position for one frame after eating a dot
                    if (_dotEatenThisFrame)
                        _dotEatenThisFrame = false;
                    else
                        player.UpdatePosition();

                    // check if all dots are eaten
                    if (_playingLevelCompleteAnimation)
                        return;
                }
            }
        }
    }

    void CheckForSpriteCollisions()
    {
        foreach (var ghost in GhostHouse.Ghosts)
        {
            if (Player.HitBox.Intersects(ghost.HitBox))
            {
                if (ghost.Mode == GhostMode.Frightened)
                {
                    OnGhostEaten(ghost);
                }
                else if (ghost.Mode != GhostMode.Eaten)
                {
                    OnPlayerCaught();
                    return;
                }
            }
        }
    }

    void TogglePause() =>
        IsPaused = !IsPaused;

    public Point GetNextPosToPlayer(Point ghostPosition)
    {
        Point destination;
        if (Player.Destination == Destination.None)
            destination = Player.Departure.Position;
        else
        {
            destination = Player.Destination.Position != ghostPosition ?
                Player.Destination.Position : Player.Departure.Position;
        }
        return GetNextPosition(ghostPosition, destination);
    }

    public Point GetNextPosToGHouse(Point ghostPosition) =>
        GetNextPosition(ghostPosition, GhostHouse.EntrancePosition);


    // returns tile position in the provided direction or the current position if the destination is not walkable
    public Point GetNextPosition(Point position, Direction direction)
    {
        if (direction == Direction.None)
            throw new ArgumentException("Direction is required.");

        if (!IsWalkable(position))
            throw new ArgumentException("Provided position is not on the board.");

        Point destination = position + direction;
        
        if (IsWalkable(destination))
            return destination;
        else
            return position;
    }

    // returns next position on the path to the destination
    public Point GetNextPosition(Point position, Point destination)
    {
        if (position == Point.None || destination == Point.None)
            throw new ArgumentException("Points cannot be None.");

        if (position == destination)
            return destination;

        var path = _aStar.ShortestPath(position, destination);
        if (path != null)
        {
            if (path.Length > 1)
                return path.GetStepWithStart(1);
            else
                return destination;
        }
        else
            throw new ArgumentException("Provided position does not produce a valid path to the destination.");
    }

    // returns direction to player
    public Direction GetDirectionToPlayer(Point ghostPosition) =>
        Direction.GetCardinalDirection(ghostPosition, Player.Destination.Position);

    public static Direction GetRandomTurn(Direction direction)
    {
        return GlobalRandom.DefaultRNG.NextInt(0, 2) switch
        {
            0 => direction + 2,
            _ => direction - 2
        };
    }

    public Dot? GetRandomDot(Rectangle area)
    {
        var dots = _renderer.Entities.Where(e => e is Dot d && area.Contains(d.Position)).ToArray();
        if (dots.Length == 0) 
            return null;
        else
        {
            int index = GlobalRandom.DefaultRNG.RandomIndex(dots);
            return dots[index] as Dot;
        }
    }

    // returns a valid position for the given area
    public Point GetRandomPosition(Rectangle area)
    {
        Point position;
        do 
            position = GlobalRandom.DefaultRNG.RandomPosition(area);
        while (!IsWalkable(position));
        return position;
    }

    // returns a valid destination as close to the desired direction as possible
    public Destination GetDestination(Point currentPosition, Direction desiredDirection, Direction currentDirection)
    {
        // desired and current directions are different
        if (desiredDirection != currentDirection)
        {
            // try the new direction
            var position = GetNextPosition(currentPosition, desiredDirection);
            if (position != currentPosition)
                return new Destination(position, desiredDirection);

            else
            {
                // try the current direction
                position = GetNextPosition(currentPosition, currentDirection);
                if (position != currentPosition)
                    return new Destination(position, currentDirection);

                else
                {
                    // try the remaining direction
                    var direction = desiredDirection.Inverse();
                    position = GetNextPosition(currentPosition, direction);
                    if (position != currentPosition)
                        return new Destination(position, direction);

                    else
                        // dead end without a valid turn? this shouldn't happen...
                        throw new InvalidOperationException("Can't find a valid destination from given directions.");
                }
            }
        }

        // desired and current directions are the same
        else
        {
            // try the current direction
            var position = GetNextPosition(currentPosition, currentDirection);
            if (position != currentPosition)
                return new Destination(position, currentDirection);

            else
            {
                // try a random turn
                var direction = GetRandomTurn(currentDirection);
                position = GetNextPosition(currentPosition, direction);
                if (position != currentPosition)
                    return new Destination(position, direction);

                else
                {
                    // try the remaining direction
                    direction = direction.Inverse();
                    position = GetNextPosition(currentPosition, direction);
                    if (position != currentPosition)
                        return new Destination(position, direction);

                    else
                        // dead end without a valid turn? this shouldn't happen...
                        throw new InvalidOperationException("Can't find a valid destination from given directions.");
                }
            }
        }
    }

    public IEdible? GetConsumable(Point position)
    {
        var entity = _renderer.Entities.Where(e => e.Position == position).FirstOrDefault();
        if (entity is Dot dot)
            return dot;
        else
            return null;
    }

    public void RemoveDot(Dot dot)
    {
        _renderer.Remove(dot);
        OnDotEaten(dot);
    }

    // adds dots to the map
    void DrawDots(Level level)
    {
        _renderer.AddRange(level.Dots);
        SadComponents.Add(_renderer);
    }

    // assigns glyphs to wall tiles
    void DrawWalls(Level level)
    {
        foreach (var tile in level.Tiles)
        {
            if (tile is Wall wall)
            {
                // find neighbouring walls
                var adjacentPoints = _adjacencyRule.Neighbors(wall.Position);
                var adjacentWalls = level.Tiles.Where(t => t is Wall && adjacentPoints.Contains(t.Position))
                    .Select(t => (Wall)t).ToList();

                // set wall glyph
                wall.SetAppearance(adjacentWalls);
            }
        }
    }

    // soft restart with the score left unchanged
    public void Restart()
    {
        // add removed dots
        if (ReplenishDotsOnLiveLost)
        {
            _renderer.AddRange(_removedDots);
            _removedDots.Clear();
        }

        // add sprites
        RemoveAll();
        Children.Add(Player);
        Children.Add(GhostHouse);

        // add a small pause at the beginning
        SadComponents.Add(new StartPause());
    }

    public void RemoveDots()
    {
        _renderer.RemoveAll();
        _renderer.IsDirty = true;
    }

    void RemoveAll()
    {
        Children.Remove(Player);
        Children.Remove(GhostHouse);
    }

    void ChangeMazeColor(Color color)
    {
        var mazeWalls = Surface.Where(cg => cg is Wall && cg is not SpawnerEntrance);
        foreach (var wall in mazeWalls)
            wall.Foreground = color;
        Surface.IsDirty = true;
    }

    void Player_OnDeathAnimationFinished(object? o, EventArgs e)
    {
        Children.Remove(Player);
        OnLiveLost();
    }

    void GhostHouse_OnPowerDotStarted(object? o, EventArgs e)
    {
        if (GamePlayIsOn())
        {
            Sounds.Siren.Stop();
            Sounds.PowerDot.Play();
        }
    }

    void GhostHouse_OnPowerDotDepleted(object? o, EventArgs e)
    {
        if (GamePlayIsOn())
        {
            Sounds.PowerDot.Stop();
            Sounds.Siren.Play();
        }
    }

    void GhostHouse_OnModeChanged(object? o, GhostModeEventArgs e)
    {
        _secondCounter = TimeSpan.Zero;
        var text = $"Prev: {e.PrevMode}, New: {e.NewMode}        ";
        _debug.Surface.Print(3, 0, text);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Game game)
        {
            Position = ((game.WidthPixels - WidthPixels) / 2,
                game.HeightPixels - HeightPixels - Convert.ToInt32(Game.DefaultFontSize.Y * 1.5d));
        }
        base.OnParentChanged(oldParent, newParent);
    }

    void OnPlayerCaught()
    {
        Children.Remove(GhostHouse);
        Player.Die();
        Sounds.StopAll();
        Sounds.Death.Play();
    }

    void OnLiveLost()
    {
        LiveLost?.Invoke(this, EventArgs.Empty);
    }

    void OnDotEaten(Dot dot)
    {
        // add the dot to removed dots if needed
        if (ReplenishDotsOnLiveLost)
            _removedDots.Add(dot);

        // mark renderer to redraw
        _renderer.IsDirty = true;

        // mark pacman to stop for one frame
        _dotEatenThisFrame = true;

        // check if all the dots are eaten
        if (_renderer.Entities.Count == 0)
            OnLevelComplete();
        else
            Sounds.MunchDot.Play();

        DotEaten?.Invoke(this, new DotEventArgs(dot));
    }

    void OnGhostEaten(Ghost ghost)
    {
        ghost.Mode = GhostMode.Eaten;
        Sounds.MunchGhost.Play();
        //SadComponents.Add(new Pause(0.2d));
        GhostEaten?.Invoke(this, new ScoreEventArgs(GhostHouse.Value));
    }

    // new board is to be created after the level is complete
    void OnLevelComplete()
    {
        _playingLevelCompleteAnimation = true;
        Children.Remove(GhostHouse);
        Sounds.StopAll();
        Sounds.LevelComplete.Play();
    }

    void OnLevelCompleteAnimationFinished()
    {
        RemoveAll();
        LevelComplete?.Invoke(this, EventArgs.Empty);
    }
    #endregion Methods

    #region Events
    public event EventHandler<DotEventArgs>? DotEaten;
    public event EventHandler<ScoreEventArgs>? GhostEaten;
    public event EventHandler? LiveLost;
    public event EventHandler? LevelComplete;
    #endregion Events
}

class ScoreEventArgs : EventArgs
{
    public int Value { get; init; }
    public ScoreEventArgs(int value) => Value = value;
}

class DotEventArgs : EventArgs
{
    public Dot Dot { get; init; }
    public DotEventArgs(Dot dot) => Dot = dot;
}