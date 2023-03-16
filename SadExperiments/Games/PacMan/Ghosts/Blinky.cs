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

        if (newParent is Board)
        {
            Mode = GhostMode.Scatter;
            if (!TrySetDestination(Direction.Right))
                throw new InvalidOperationException("Invalid Blinky start location.");
            SetCurrentAnimationGlyph();
        }
    }
}