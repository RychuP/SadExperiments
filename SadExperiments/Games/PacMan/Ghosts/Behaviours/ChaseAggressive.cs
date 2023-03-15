namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ChaseAggressive : IChaseBehaviour
{
    public Destination Chase(Board board, Destination prevDestination)
    {
        var nextPosition = board.GetNextPosToPlayer(prevDestination.Position);
        var desiredDirection = Direction.GetCardinalDirection(prevDestination.Position, nextPosition);

        // check astar direction
        if (desiredDirection != prevDestination.Direction.Inverse() && desiredDirection != Direction.None)
            return new Destination(nextPosition, desiredDirection);

        // find own direction
        else
        {
            desiredDirection = board.GetDirectionToPlayer(prevDestination.Position);
            if (desiredDirection == prevDestination.Direction.Inverse())
                desiredDirection = Board.GetRandomTurn(prevDestination.Direction);
            return board.GetDestination(prevDestination.Position, desiredDirection, prevDestination.Direction);
        }
    }
}