using SadConsole.Entities;
using SadConsole.Instructions;
using SadExperiments.Games.PacMan.Ghosts;

namespace SadExperiments.Games.PacMan;

class Board : ScreenSurface
{
    readonly AdjacencyRule _adjacencyRule = AdjacencyRule.EightWay;
    readonly Renderer _renderer = new();
    readonly GhostHouse _ghostHouse;
    bool _isPaused = true;
    int _score;

    // sprites
    readonly Player _player;
    readonly Blinky _blinky;
    readonly Inky _inky;
    readonly Pinky _pinky;
    readonly Clyde _clyde;

    public Board(Level level, Player player) : base(level.Width, level.Height, level.Tiles)
    {
        UsePixelPositioning = true;
        Position = (1, 2);
        Font = Fonts.Maze;
        FontSize = Game.DefaultFontSize;
        _player = player;

        // draw board
        DrawWalls(level);
        DrawDots(level);

        // create ghost house
        var spawner = level.Tiles.Where(t => t is Spawner).FirstOrDefault();
        if (spawner != null)
            _ghostHouse = new(spawner.Position);
        else
            throw new ArgumentException("Tiles do not container a spawner location.");

        // create ghosts
        _blinky = new(_ghostHouse.BlinkyPosition);
        _pinky = new(_ghostHouse.PinkyPosition);
        _inky = new(_ghostHouse.InkyPosition);
        _clyde = new(_ghostHouse.ClydePosition);

        // assign start points
        _player.Start = level.Start.SurfaceLocationToPixel(FontSize);
        Children.Add(_pinky, _inky, _clyde, _blinky, player);

        // reset score
        Score = 0;

        // add a small pause at the beginning
        SadComponents.Add(new Pause());
    }

    public int Score
    {
        get => _score;
        private set
        {
            _score = value;
            OnScoreChanged();
        }
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysDown)
        {
            if (keyboard.IsKeyDown(Keys.Right))
            {
                _player.NextDirection = Direction.Right;
                return true;
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                _player.NextDirection = Direction.Left;
                return true;
            }
            else if (keyboard.IsKeyDown(Keys.Up))
            {
                _player.NextDirection = Direction.Up;
                return true;
            }
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                _player.NextDirection = Direction.Down;
                return true;
            }
        }
        return base.ProcessKeyboard(keyboard);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        base.OnParentChanged(oldParent, newParent);
        if (newParent is Game game)
        {
            Sounds.Siren.Play();
            Sounds.Siren.IsLooped = true;

            Position = ((game.WidthPixels - WidthPixels) / 2,
                game.HeightPixels - HeightPixels - Convert.ToInt32(Game.DefaultFontSize.Y * 1.5d));
        }
    }

    public override void Update(TimeSpan delta)
    {
        if (!_isPaused)
        {
            foreach (var child in Children)
            {
                if (child is Sprite actor)
                    actor.UpdatePosition();
            }
        }
        base.Update(delta);
    }

    public void TogglePause() { _isPaused = !_isPaused; }

    public Point GetNextPosition(Point currentPosition, Direction direction)
    {
        Point surfacePosition = currentPosition.PixelLocationToSurface(FontSize);

        if (direction == Direction.None)
            return currentPosition;

        Point nextPosition = surfacePosition + direction;
        if (IsWalkable(nextPosition))
            return nextPosition.SurfaceLocationToPixel(FontSize);
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
        Score += dot.Value;
        _renderer.Remove(dot);
        _renderer.IsDirty = true;
    }

    // checks if the position is valid and walkable
    bool IsWalkable(Point position) =>
        Surface.Area.Contains(position) && Surface[position.ToIndex(Surface.Width)] is Floor;

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

    void OnScoreChanged()
    {
        ScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? ScoreChanged;
}

class Pause : InstructionSet
{
    public Pause()
    {
        RemoveOnFinished = true;
        Instructions.AddFirst(new Wait(TimeSpan.FromSeconds(1)));
        Instructions.AddLast(new CodeInstruction((o, t) =>
        {
            if (o is Board b) b.TogglePause();
            return true;
        }));
    }
}