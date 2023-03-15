namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

abstract class ScatterBase : IScatterBehaviour
{
    const double MinimalDistanceBetweenPatrolDots = 4d;

    protected Rectangle Area { get; init; } = Rectangle.Empty;

    protected Point Destination { get; set; } = Point.None;

    public ScatterBase(Ghost host)
    {
        host.ModeChanged += Ghost_OnModeChanged;
    }

    public virtual Destination Scatter(Board board, Destination prevDestination)
    {
        // destination is not set
        if (Destination == Point.None)
        {
            Destination = prevDestination.Position;
            SetDestination(board);
        }

        // destination is reached
        else if (Destination == prevDestination.Position)
            SetDestination(board);

        var nextPosition = board.GetNextPosition(prevDestination.Position, Destination);
        var desiredDirection = Direction.GetCardinalDirection(prevDestination.Position, nextPosition);

        // check astar direction
        if (desiredDirection != prevDestination.Direction.Inverse() && desiredDirection != Direction.None)
            return new Destination(nextPosition, desiredDirection);

        // find own direction
        else
        {
            if (desiredDirection == prevDestination.Direction.Inverse())
                desiredDirection = Board.GetRandomTurn(prevDestination.Direction);
            else if (desiredDirection == Direction.None)
                desiredDirection = prevDestination.Direction;
            return board.GetDestination(prevDestination.Position, desiredDirection, prevDestination.Direction);
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