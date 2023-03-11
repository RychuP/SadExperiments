using GoRogue.Pathing;
using SadConsole.Entities;
using SadExperiments.Games.PacMan.Ghosts;
using SadRogue.Primitives.GridViews;

namespace SadExperiments.Games.PacMan;

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
        Sounds.Siren.IsLooped = true;

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
                    OnPlayerCaught();
                    return;
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
        if (path != null && path.Length > 1)
            return path.GetStepWithStart(1).SurfaceLocationToPixel(FontSize);
        else
            return currentGhostPosition;
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
        Children.Clear();
        Children.Add(Player);
        Children.Add(GhostHouse.Ghosts);

        // add a small pause at the beginning
        SadComponents.Add(new Pause());

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

    void OnGhostEaten(int value)
    {
        GhostEaten?.Invoke(this, new ScoreEventArgs(value));
    }

    void OnLevelComplete()
    {
        _playingLevelCompleteAnimation = true;
        Player.AnimationIsOn = false;
        RemoveGhosts();
        Sounds.StopAll();
        Sounds.LevelComplete.Play();
    }

    void OnLevelCompleteAnimationFinished()
    {
        Children.Clear();
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