namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class WakenUpBehaviour : IAwakeBehaviour
{
    public Destination LeaveHouse(Board board, Point position)
    {
        var house = board.GhostHouse;
        var nextPosition = position == house.ClydePosition || 
                position == house.InkyPosition ? 
                    house.PinkyPosition : house.EntrancePosition;

        if (nextPosition == house.PinkyPosition)
        {
            var direction = Direction.GetCardinalDirection(position, nextPosition);
            return new Destination(house.PinkyPosition, direction);
        }
        else
            return new Destination(house.EntrancePosition, Direction.Up);
    }
}