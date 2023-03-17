namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ChaseAhead : ChaseBaseBehaviour
{
    public override Destination Chase(Board board, Point position, Direction direction)
    {
        // find a valid position up to 4 tiles in front of the player
        Point ambushPosition = board.GetPlayerPosition();
        var playerDirection = board.Player.Destination.Direction;
        if (playerDirection != Direction.None)
        {
            for (int i = 0; i < 4; i++)
            {
                var testPosition = ambushPosition + playerDirection;
                if (board.IsWalkable(testPosition))
                    ambushPosition = testPosition;
                else
                    break;
            }
        }

        if (board.IsDebugging)
        {
            //board.HighlightTile(ambushPosition);
        }

        return Navigate(board, position, direction, ambushPosition);
    }
}