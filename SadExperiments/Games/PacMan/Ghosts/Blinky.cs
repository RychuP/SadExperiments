using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Blinky : Ghost
{
    public Blinky(Board board, Point start) : base(board, start)
    {
        AnimationRow = 2;
        ChaseBehaviour = new ChaseAggressive();
        ScatterBehaviour = new ScatterTopRightCorner(board.Surface.Area, this);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        base.OnParentChanged(oldParent, newParent);

        // create scatter behaviour once before the board is assigned to a game
        if (newParent is Board)
        {
            Mode = GhostMode.Scatter;
            if (!TrySetDestination(Direction.Right))
                throw new InvalidOperationException("Invalid Blinky start position.");
            SetCurrentAnimationGlyph();
        }
    }

    protected override void OnCurrentPositionChanged(Point prevPosition, Point newPosition)
    {
        base.OnCurrentPositionChanged(prevPosition, newPosition);
    }
}