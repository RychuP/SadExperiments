namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ChaseAggressive : IChaseBehaviour
{
    public Destination Chase(Board board, Point ghostPosition, Direction ghostDirection)
    {
        var positionLeadingToPlayer = board.GetPositionToPlayer(ghostPosition);
        var desiredDirection = Direction.GetDirection(ghostPosition, positionLeadingToPlayer);

        // check astar direction
        if (desiredDirection != ghostDirection.Inverse() && desiredDirection != Direction.None)
            return new Destination(positionLeadingToPlayer, desiredDirection);

        // find own direction
        else
        {
            desiredDirection = board.GetDirectionToPlayer(ghostPosition);
            if (desiredDirection == ghostDirection.Inverse())
                desiredDirection = Board.GetRandomTurn(ghostDirection);
            return board.GetDestination(ghostPosition, desiredDirection, ghostDirection);
        }
    }
}