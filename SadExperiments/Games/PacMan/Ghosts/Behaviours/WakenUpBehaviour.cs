namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class WakenUpBehaviour : IAwakeBehaviour
{
    public Destination LeaveHouse(Board board, Point ghostPosition)
    {
        var house = board.GhostHouse;
        var nextPosition = ghostPosition == house.Clyde.Position || ghostPosition == house.Inky.Position ? 
            house.CenterPosition : house.EntrancePosition;

        if (nextPosition == house.CenterPosition)
        {
            var direction = Direction.GetCardinalDirection(ghostPosition, nextPosition);
            return new Destination(house.CenterPosition, direction);
        }
        else
            return new Destination(house.CenterPosition, Direction.None);
    }
}