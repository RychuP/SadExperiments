using GoRogue.Pathing;
using GoRogue.Random;
using SadConsole.Entities;
using SadExperiments.Games.PacMan.Ghosts;
using SadExperiments.Games.PacMan.Ghosts.Behaviours;
using SadRogue.Primitives.GridViews;

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
    readonly List<Dot> _removedDots;
    readonly Renderer _renderer = new();
    readonly AStar _aStar;
    bool _isPaused = true;
    bool _playingLevelCompleteAnimation = false;

    // level complete animation
    readonly TimeSpan _colorChangeDuration = TimeSpan.FromSeconds(0.33d);
    TimeSpan _animationTimeDelta = TimeSpan.Zero;
    const int WallColorChangeMax = 6;
    int _wallColorChangeCount = 0;
    #endregion Fields

    #region Constructors
    public Board(Level level) : base(level.Width, level.Height, level.Tiles)
    {
        // setup
        UsePixelPositioning = true;
        Position = (1, 2);
        Font = Fonts.Maze;
        FontSize = Game.DefaultFontSize;

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
            GhostHouse = new(spawner.Position);
        else
            throw new ArgumentException("Tiles do not contain a spawner location.");
        Children.Add(GhostHouse);

        // player
        Player = new Player(level.Start.SurfaceLocationToPixel(FontSize));
        Player.DeathAnimationFinished += Player_OnDeathAnimationFinished;
        Children.Add(Player);

        // ghosts
        Children.Add(GhostHouse.Ghosts);

        // small pause at the beginning
        SadComponents.Add(new Pause());
    }
    #endregion Constructors

    #region Properties
    public Player Player { get; init; }
    public GhostHouse GhostHouse { get; init; }
    #endregion Properties

    #region Methods
    // checks if the position is valid and walkable
    public bool IsWalkable(Point position) =>
        Surface.Area.Contains(position) && Surface[position.ToIndex(Surface.Width)] is Floor;

    public bool IsPortal(Point pixelPosition, out Portal? destination)
    {
        var surfacePosition = pixelPosition.PixelLocationToSurface(FontSize);

        if (Surface.Area.Contains(surfacePosition))
        {
            int index = surfacePosition.ToIndex(Surface.Width);
            if (Surface[index] is Portal portal)
            {
                destination = Surface.Where(cg => cg is Portal p
                    && p.Position != surfacePosition
                    && p.Id == portal.Id).FirstOrDefault() as Portal;
                if (destination is null)
                    throw new ArgumentException($"Map doesn't contain a destination portal for id: {portal.Id}");
                return true;
            }
        }

        destination = null;
        return false;
    }

    bool GamePlayIsOn() =>
        Children.Contains(Player) && !_isPaused && !_playingLevelCompleteAnimation && !Player.IsDead;

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed && keyboard.IsKeyPressed(Keys.P))
        {
            TogglePause();
        }
        else if (GamePlayIsOn() && keyboard.HasKeysDown)
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
            // update all sprite positions
            foreach (var child in Children)
                if (child is Sprite actor)
                {
                    actor.UpdatePosition();
                    if (_playingLevelCompleteAnimation) return;
                }

            // check for collisions
            foreach (var child in Children)
            {
                if (child is Ghost ghost && Player.HitBox.Intersects(ghost.HitBox))
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
        else if (_playingLevelCompleteAnimation)
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
        base.Update(delta);
    }

    public void TogglePause() =>
        _isPaused = !_isPaused;

    public Point GetPositionToPlayer(Point currentGhostPosition)
    {
        Point toPosition = Player.ToPosition != currentGhostPosition ? Player.ToPosition : Player.FromPosition;
        toPosition = toPosition.PixelLocationToSurface(FontSize);
        Point ghostPosition = currentGhostPosition.PixelLocationToSurface(FontSize);
        var path = _aStar.ShortestPath(ghostPosition, toPosition);
        if (path != null)
        {
            if (path.Length > 1)
                return path.GetStepWithStart(1).SurfaceLocationToPixel(FontSize);
            else
                return currentGhostPosition;
        }
        else
            throw new ArgumentException("Given ghost position does not produce a valid path to the player");
    }

    public Point GetPositionToGhostHouse(Point currentGhostPosition)
    {
        var position = currentGhostPosition.PixelLocationToSurface(FontSize);
        var destination = GhostHouse.EntrancePosition.PixelLocationToSurface(FontSize);
        var path = _aStar.ShortestPath(position, destination);
        if (path != null)
        {
            if (path.Length > 1)
                return path.GetStepWithStart(1).SurfaceLocationToPixel(FontSize);
            else
                return GhostHouse.EntrancePosition;
        }
        else
            throw new ArgumentException("Given ghost position does not produce a valid path to the ghost house.");
    }

    // returns direction to player
    public Direction GetDirectionToPlayer(Point ghostPosition) =>
        Direction.GetCardinalDirection(ghostPosition, Player.ToPosition);

    public static Direction GetRandomTurn(Direction direction)
    {
        return GlobalRandom.DefaultRNG.NextInt(0, 2) switch
        {
            0 => direction + 2,
            _ => direction - 2
        };
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

    public Point GetNextPosition(Point currentPosition, Direction direction)
    {
        Point surfacePosition = currentPosition.PixelLocationToSurface(FontSize);

        if (direction == Direction.None)
            return currentPosition;

        Point position = surfacePosition + direction;
        // check maze locations
        if (IsWalkable(position))
            return position.SurfaceLocationToPixel(FontSize);
        else
            return currentPosition;
    }

    public IEdible? GetConsumable(Point pixelPosition)
    {
        Point surfacePosition = pixelPosition.PixelLocationToSurface(FontSize);
        var entity = _renderer.Entities.Where(e => e.Position == surfacePosition).FirstOrDefault();
        if (entity is Dot dot)
            return dot;
        else
            return null;
    }

    public void RemoveDot(Dot dot)
    {
        _renderer.Remove(dot);
        _removedDots.Add(dot);
        _renderer.IsDirty = true;
        OnDotEaten(dot);
        if (_renderer.Entities.Count == 0)
            OnLevelComplete();
        else
            Sounds.MunchDot.Play();
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
        _isPaused = true;

        // add dots
        _renderer.AddRange(_removedDots);
        _removedDots.Clear();

        // add sprites
        RemoveSprites();
        Children.Add(Player);
        Children.Add(GhostHouse.Ghosts);

        // add a small pause at the beginning
        SadComponents.Add(new Pause());

        Sounds.StopAll();
        Sounds.Siren.Play();
    }

    public void RemoveDots()
    {
        _renderer.RemoveAll();
    }

    void RemoveGhosts()
    {
        foreach (var ghost in GhostHouse.Ghosts)
            Children.Remove(ghost);
    }

    void RemoveSprites()
    {
        RemoveGhosts();
        Children.Remove(Player);
    }

    void ChangeMazeColor(Color color)
    {
        var mazeWalls = Surface.Where(cg => cg is Wall && cg is not SpawnerEntrance);
        foreach (var wall in mazeWalls)
            wall.Foreground = color;
        Surface.IsDirty = true;
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        base.OnParentChanged(oldParent, newParent);
        if (newParent is Game game)
        {
            Sounds.Siren.Play();
            Position = ((game.WidthPixels - WidthPixels) / 2,
                game.HeightPixels - HeightPixels - Convert.ToInt32(Game.DefaultFontSize.Y * 1.5d));
        }
    }

    void Player_OnDeathAnimationFinished(object? o, EventArgs e)
    {
        Children.Remove(Player);
        OnLiveLost();
    }

    void OnPlayerCaught()
    {
        RemoveGhosts();
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
        Player.AnimationIsOn = false;
        RemoveGhosts();
        Children.Remove(GhostHouse);
        Sounds.StopAll();
        Sounds.LevelComplete.Play();
    }

    void OnLevelCompleteAnimationFinished()
    {
        RemoveSprites();
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