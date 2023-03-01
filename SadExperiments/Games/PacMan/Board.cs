using SadConsole.Entities;

namespace SadExperiments.Games.PacMan;

class Board : ScreenSurface
{
    readonly AdjacencyRule _adjacencyRule = AdjacencyRule.EightWay;
    readonly Renderer _renderer = new();

    public Board(Level level) : base(level.Width, level.Height, level.Tiles)
    {
        Position = (1, 2);
        Font = Fonts.Maze;
        FontSize *= 2;

        // get perimeter positions
        var perimeter = Surface.Area.PerimeterPositions();

        // iterate through tiles to find all walls
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

        _renderer.AddRange(level.Dots);
        SadComponents.Add(_renderer);
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