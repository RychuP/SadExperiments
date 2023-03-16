namespace SadExperiments.Games.PacMan.Ghosts.Behaviours
{
    internal class ChaseBaseBehaviour : BaseBehaviour, IChaseBehaviour
    {
        public virtual Destination Chase(Board board, Point position, Direction direction) =>
            Destination.None;

        protected static Destination Navigate(Board board, Point position, Direction direction, Point destination)
        {
            if (position == board.GhostHouse.CenterSpot)
                return new Destination(board.GhostHouse.EntrancePosition, Direction.Up);

            var nextPosition = board.GetNextPosition(position, destination);
            var desiredDirection = Direction.GetCardinalDirection(position, nextPosition);

            // check astar direction
            if (desiredDirection != direction.Inverse() && desiredDirection != Direction.None)
                return new Destination(nextPosition, desiredDirection);

            // find own direction
            else
            {
                desiredDirection = Direction.GetCardinalDirection(position, nextPosition);
                if (desiredDirection == direction.Inverse())
                    desiredDirection = Board.GetRandomTurn(direction);
                else if (desiredDirection == Direction.None)
                    desiredDirection = direction;
                return board.GetDestination(position, desiredDirection, direction);
            }
        }
    }
}