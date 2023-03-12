namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class EatenRunningHome : IEatenBehaviour
{
    public Destination RunBackHome(Board board, Point ghostPosition, Direction ghostDirection)
    {
        // ghost reached the house center
        if (ghostPosition == board.GhostHouse.CenterPosition)
            return new Destination(board.GhostHouse.CenterPosition, Direction.None);

        // ghost is at the house entrance
        else if (ghostPosition == board.GhostHouse.EntrancePosition)
            return new Destination(board.GhostHouse.CenterPosition, Direction.Down);

        // ghost is away from the house
        else
        {
            var nextPosition = board.GetPositionToGhostHouse(ghostPosition);
            var direction = Direction.GetDirection(ghostPosition, nextPosition);
            return new Destination(nextPosition, direction);
        }
    }
}