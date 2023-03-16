namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class EatenRunningHome : IEatenBehaviour
{
    public Destination RunBackHome(Board board, Point position)
    {
        // ghost reached the house center
        if (position == board.GhostHouse.CenterSpot)
            return new Destination(board.GhostHouse.EntrancePosition, Direction.Up);

        // ghost is at the house entrance
        else if (position == board.GhostHouse.EntrancePosition)
            return new Destination(board.GhostHouse.CenterSpot, Direction.Down);

        // ghost is away from the house
        else
        {
            var nextPosition = board.GetNextPosToGHouse(position);
            var direction = Direction.GetDirection(position, nextPosition);
            return new Destination(nextPosition, direction);
        }
    }
}