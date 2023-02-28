namespace SadExperiments.Games.PacMan;

class Board : ScreenSurface
{
    readonly AdjacencyRule _adjacencyRule = AdjacencyRule.EightWay;

    public Board(int width, int height, Tile[] tiles) : base(width, height, tiles)
    {
        Position = (1, 2);
        Font = Fonts.Maze;
        FontSize *= 2;

        // get perimeter positions
        var perimeter = Surface.Area.PerimeterPositions();

        // iterate through tiles to find all walls
        foreach (var tile in tiles)
        {
            
            if (tile is Wall wall)
            {
                // find neighbouring walls
                var adjacentPoints = _adjacencyRule.Neighbors(wall.Position);
                var adjacentWalls = tiles.Where(t => t is Wall && adjacentPoints.Contains(t.Position))
                    .Select(t => (Wall)t).ToList();

                // check if the wall is on the perimeter
                bool isPerimeter = perimeter.Contains(wall.Position);
                PerimeterWall pw = PerimeterWall.None;

                if (isPerimeter)
                {
                    // fake wall index
                    int x = wall.Position.X;
                    int y = wall.Position.Y;

                    int index = x == 0 & y == 0 ? 0 :                       // top left corner
                                x == 0 & y == height - 1 ? 0 :
                                x == width - 1 && y == 0 ? 0 :
                                x == width - 1 && y == height - 1 ? 0 :
                                x == 0 ? 3 :                                // left wall
                                x == width - 1 ? 5 :                        // right wall
                                y == 0 ? 1 :                                // top wall
                                y == height - 1 ? 7 :                       // bottom wall
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