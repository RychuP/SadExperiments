namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

abstract class ScatterBase : IScatterBehaviour
{
    const double MinimalDistanceBetweenPatrolDots = 4d;

    protected Rectangle Area { get; init; } = Rectangle.Empty;

    protected Point ToPosition { get; set; } = Point.None;

    public virtual Destination Scatter(Board board, Point ghostPosition, Direction ghostDirection)
    {
        if (ToPosition == Point.None)
        {
            ToPosition = ghostPosition;
            SetToPosition(board);
        }
        else if (ToPosition == ghostPosition)
            SetToPosition(board);

        var nextPosition = board.GetNextPosition(ghostPosition, ToPosition);
        var desiredDirection = Direction.GetCardinalDirection(ghostPosition, nextPosition);

        // check astar direction
        if (desiredDirection != ghostDirection.Inverse() && desiredDirection != Direction.None)
            return new Destination(nextPosition, desiredDirection);

        // find own direction
        else
        {
            if (desiredDirection == ghostDirection.Inverse())
                desiredDirection = Board.GetRandomTurn(ghostDirection);
            else if (desiredDirection == Direction.None)
                desiredDirection = ghostDirection;
            return board.GetDestination(ghostPosition, desiredDirection, ghostDirection);
        }
    }

    protected void SetToPosition(Board board)
    {
        if (Area == Rectangle.Empty)
            throw new InvalidOperationException("Area is not set.");

        // on the second and subsequent runs try patrolling around dots if they are not too close to each other
        if (ToPosition != Point.None)
        {
            Dot? dot = board.GetRandomDot(Area);
            if (dot != null && Distance.Chebyshev.Calculate(dot.Position, ToPosition) > MinimalDistanceBetweenPatrolDots)
            {
                ToPosition = dot.Position.SurfaceLocationToPixel(board.FontSize);
                return;
            }
        }

        // find a random position that is different than current ToPosition
        Point position;
        do
            position = board.GetRandomPosition(Area);
        while (ToPosition != position);

        // change destination surface to pixel position
        ToPosition = position.SurfaceLocationToPixel(board.FontSize);
    }

    protected void Ghost_OnModeChanged(object? o, GhostModeEventArgs e)
    {
        if (e.PrevMode == GhostMode.Scatter)
            ToPosition = Point.None;
    }
}