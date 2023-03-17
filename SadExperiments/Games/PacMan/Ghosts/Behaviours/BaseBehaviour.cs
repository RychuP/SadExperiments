namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class BaseBehaviour
{
    // destination where the ghost is going
    protected Point ToPosition { get; set; } = Point.None;

    protected virtual void Ghost_OnModeChanged(object? o, GhostModeEventArgs e) =>
        ToPosition = Point.None;

    // returns a random valid position in the given area that is different than current destination
    protected Point GetRandValidPosInArea(Board board, Rectangle area)
    {
        Point position;
        do position = board.GetRandomPosition(area);
        while (ToPosition != position && !board.IsReachable(position));
        return position;
    }
}