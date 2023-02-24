using GoRogue.GameFramework;
using SadRogue.Primitives.GridViews;

namespace SadExperiments.Games.PacMan;

class Board : Map
{
    readonly AdjacencyRule _adjacencyRule = AdjacencyRule.Cardinals;
    public ScreenSurface Appearance { get; init; }

    public Board(int w, int h, Tile[] tiles) : base(w, h, 1, Distance.Manhattan)
    {
        Appearance = new ScreenSurface(w, h)
        {
            Position = (1, 1),
            Font = Fonts.Maze
        };
        Appearance.FontSize *= 2;

        // cross road
        // _|   |_
        //   0 1
        // _ 2 3 _
        //  |   |

        foreach (var tile in tiles)
        {
            if (tile is not Wall wall) continue;

            var points = _adjacencyRule.Neighbors(wall.Position);
            var neighbourWalls = tiles.Where(t => t is Wall && points.Contains(t.Position));

            // there is a wall above
            if (neighbourWalls.Any(t => t.Position.Y < wall.Position.Y))
            {
                // there is a wall to the left
                if (neighbourWalls.Any(t => t.Position.X < wall.Position.X))
                {

                }
            }

            // no wall above, but there is a wall below
            else if (neighbourWalls.Any(t => t.Position.Y > wall.Position.Y))
            {
                // there is a wall to the left
                if (neighbourWalls.Any(t => t.Position.X < wall.Position.X))
                {
                    // wall to the left and right
                    if (neighbourWalls.Any(t => t.Position.X > wall.Position.X))
                    {
                        wall.Appearance.Glyph = 4;
                        wall.Appearance.Mirror = Mirror.Vertical;
                        //wall.Appearance.
                    }
                }
                // no wall on the left, but there is a wall to the right
                else if (neighbourWalls.Any(t => t is Wall && t.Position.X > wall.Position.X))
                {
                    wall.Appearance.Glyph = 0;
                    wall.Appearance.Mirror = Mirror.Horizontal;
                }
            }

            // no walls above and below
            else
            {

            }
                
        }
    }
}
