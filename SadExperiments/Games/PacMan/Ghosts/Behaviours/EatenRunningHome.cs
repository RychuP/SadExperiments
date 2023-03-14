namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class EatenRunningHome : IEatenBehaviour
{
    public Destination RunBackHome(Board board, Destination prevDestination)
    {
        // ghost reached the house center
        if (prevDestination.Position == board.GhostHouse.PinkyPosition)
            return new Destination(board.GhostHouse.EntrancePosition, Direction.Up);

        // ghost is at the house entrance
        else if (prevDestination.Position == board.GhostHouse.EntrancePosition)
            return new Destination(board.GhostHouse.PinkyPosition, Direction.Down);

        // ghost is away from the house
        else
        {
            var nextPosition = board.GetNextPosToGHouse(prevDestination.Position);
            var direction = Direction.GetDirection(prevDestination.Position, nextPosition);
            return new Destination(nextPosition, direction);
        }
    }
}