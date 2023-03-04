using SadConsole.Components;
using SadConsole.Entities;

namespace SadExperiments.Games.PacMan;

class Board : ScreenSurface
{
    readonly AdjacencyRule _adjacencyRule = AdjacencyRule.EightWay;
    readonly Renderer _renderer = new();
    readonly Timer _timer;
    readonly Player _player;
    readonly Point _playerStart;

    public Board(Level level, Player player) : base(level.Width, level.Height, level.Tiles)
    {
        Position = (1, 2);
        Font = Fonts.Maze;
        FontSize = Game.DefaultFontSize;

        // draw board
        DrawWalls(level);
        DrawDots(level);

        // save start points
        _playerStart = level.Start.SurfaceLocationToPixel(FontSize);

        // spawn actors
        _player = player;
        Children.Add(player);

        // start timer
        _timer = new(TimeSpan.FromMilliseconds(1));
        _timer.TimerElapsed += Timer_OnTimerElapsed;
        SadComponents.Add(_timer);
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

    void Timer_OnTimerElapsed(object? o, EventArgs e)
    {
        foreach (var child in Children)
        {
            if (child is Sprite actor)
                actor.UpdatePosition();
        }
    }

    public Point GetStartPosition(Sprite sprite)
    {
        return sprite switch
        {
            Player => _playerStart,
            _ => _playerStart
        };
    }

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
        var perimeter = Surface.Area.PerimeterPositions();
        foreach (var tile in level.Tiles)
        {
            if (tile is Wall wall)
            {
                // find neighbouring walls
                var adjacentPoints = _adjacencyRule.Neighbors(wall.Position);
                var adjacentWalls = level.Tiles.Where(t => t is Wall && adjacentPoints.Contains(t.Position))
                    .Select(t => (Wall)t).ToList();

                // check if the wall is on the perimeter
                bool isPerimeter = perimeter.Contains(wall.Position);
                PerimeterWall pw = PerimeterWall.None;

                if (isPerimeter)
                {
                    int x = wall.Position.X;
                    int y = wall.Position.Y;

                    int index = x == 0 & y == 0 ? 0 :                               // top left corner
                                x == 0 & y == level.Height - 1 ? 0 :
                                x == level.Width - 1 && y == 0 ? 0 :
                                x == level.Width - 1 && y == level.Height - 1 ? 0 :
                                x == 0 ? 3 :                                        // left wall
                                x == level.Width - 1 ? 5 :                          // right wall
                                y == 0 ? 1 :                                        // top wall
                                y == level.Height - 1 ? 7 :                         // bottom wall
                                4;

                    pw = index switch
                    {
                        0 => PerimeterWall.Corner,
                        1 => PerimeterWall.Top,
                        3 => PerimeterWall.Left,
                        5 => PerimeterWall.Right,
                        7 => PerimeterWall.Bottom,
                        _ => PerimeterWall.None,
                    };
                }

                // set wall neighbours and change glyph
                wall.SetAppearance(adjacentWalls, pw);
            }
        }
    }
}

enum PerimeterWall
{
    None,
    Corner,
    Top,
    Bottom,
    Left,
    Right,
}