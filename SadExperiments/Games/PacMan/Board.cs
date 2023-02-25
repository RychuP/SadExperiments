namespace SadExperiments.Games.PacMan;

class Board : ScreenSurface
{
    readonly AdjacencyRule _adjacencyRule = AdjacencyRule.EightWay;

    public Board(int width, int height, Tile[] tiles) : base(width, height, tiles)
    {
        Position = (2, 2);
        Font = Fonts.Maze;
        FontSize *= 4;

        // get perimeter positions
        var perimeter = Surface.Area.PerimeterPositions();

        foreach (var tile in tiles)
        {
            if (tile is Wall wall)
            {
                // find neighbouring walls
                var adjacentPoints = _adjacencyRule.Neighbors(wall.Position);
                var adjacentWalls = tiles.Where(t => t is Wall && adjacentPoints.Contains(t.Position)).Select(t => (Wall)t);

                // check if the wall is on the perimeter
                bool isPerimeter = perimeter.Contains(wall.Position);

                // set wall neighbours and change glyph
                wall.SetAppearance(adjacentWalls, isPerimeter);
            }
        }
    }
}