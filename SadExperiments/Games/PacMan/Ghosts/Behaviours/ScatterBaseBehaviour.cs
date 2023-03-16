namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

abstract class ScatterBaseBehaviour : IScatterBehaviour
{
    const double MinimalDistanceBetweenPatrolDots = 4d;

    protected Rectangle Area { get; init; } = Rectangle.Empty;

    protected Point Destination { get; set; } = Point.None;

    public ScatterBaseBehaviour(Ghost host)
    {
        host.ModeChanged += Ghost_OnModeChanged;
    }

    public virtual Destination Scatter(Board board, Point position, Direction direction)
    {
        if (position == board.GhostHouse.CenterSpot)
        {
            Destination = board.GhostHouse.EntrancePosition;
            return new Destination(board.GhostHouse.EntrancePosition, Direction.Up);
        }

        // destination is not set
        if (Destination == Point.None)
        {
            Destination = position;
            SetDestination(board);
        }

        // destination is reached
        else if (Destination == position)
            SetDestination(board);

        var nextPosition = board.GetNextPosition(position, Destination);
        var desiredDirection = Direction.GetCardinalDirection(position, nextPosition);

        // check astar direction
        if (desiredDirection != direction.Inverse() && desiredDirection != Direction.None)
            return new Destination(nextPosition, desiredDirection);

        // find own direction
        else
        {
            if (desiredDirection == direction.Inverse())
                desiredDirection = Board.GetRandomTurn(direction);
            else if (desiredDirection == Direction.None)
                desiredDirection = direction;
            return board.GetDestination(position, desiredDirection, direction);
        }
    }

    protected void SetDestination(Board board)
    {
        if (Area == Rectangle.Empty)
            throw new InvalidOperationException("Area is not set.");

        // try patrolling around dots if they are not too close to each other
        Dot? dot = board.GetRandomDot(Area);
        if (dot != null && Distance.Chebyshev.Calculate(dot.Position, Destination) > MinimalDistanceBetweenPatrolDots)
        {
            Destination = dot.Position;
            return;
        }

        //find a random position that is different than current destination
        Point position;
        do
            position = board.GetRandomPosition(Area);
        while (Destination != position && !board.IsReachable(position));

        // change destination to new position
        Destination = position;
    }

    protected void Ghost_OnModeChanged(object? o, GhostModeEventArgs e)
    {
        if (e.PrevMode == GhostMode.Scatter)
            Destination = Point.None;
    }
}