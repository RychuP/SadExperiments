using GoRogue;

namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ChaseDiagonal : ChaseBaseBehaviour
{
    readonly Point[] _vector = new Point[4];

    // finds a valid position up to 4 tiles ahead of the player using Blinky's vector
    public override Destination Chase(Board board, Point position, Direction direction)
    {
        Ghost blinky = board.GhostHouse.Blinky;
        Point playerPosition = board.GetPlayerPosition();
        Point ambushPosition = playerPosition;
        
        if (board.Player.Destination.Direction != Direction.None && blinky.Mode != GhostMode.Eaten)
        {
            // get the vector points between blinky and the player
            Point blinkyPosition = blinky.Destination.Position;
            var blinkyVector = Lines.Get(blinkyPosition, playerPosition).ToArray();
            Point difference = playerPosition - blinkyPosition;

            if (blinkyVector.Length > 1)
            {
                // calculate 4 points from the player that match the blinky vector
                for (int i = 0; i < 4; i++)
                {
                    if (i < blinkyVector.Length)
                        _vector[i] = blinkyVector[i] + difference;
                    else
                        _vector[i] = Point.None;
                }

                // get the last reachable point from transformed blinky vector
                for (int i = 3; i >= 0; i--)
                {
                    Point testPos = _vector[i];
                    if (testPos != Point.None && board.IsWalkable(testPos) && board.IsReachable(testPos))
                    {
                        ambushPosition = testPos;
                        break;
                    }
                }
            }
        }

        if (board.IsDebugging)
        {
            board.HighlightTile(ambushPosition);
        }

        return Navigate(board, position, direction, ambushPosition);
    }
}