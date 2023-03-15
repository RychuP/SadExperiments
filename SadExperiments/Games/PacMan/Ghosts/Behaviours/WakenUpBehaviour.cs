namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class WakenUpBehaviour : IAwakeBehaviour
{
    public Destination LeaveHouse(Board board, Point position)
    {
        var house = board.GhostHouse;
        var nextPosition = position == house.RightSpot || 
                position == house.LeftSpot ? 
                    house.CenterSpot : house.EntrancePosition;

        if (nextPosition == house.CenterSpot)
        {
            var direction = Direction.GetCardinalDirection(position, nextPosition);
            return new Destination(house.CenterSpot, direction);
        }
        else
            return new Destination(house.EntrancePosition, Direction.Up);
    }
}