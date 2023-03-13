using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Inky : Ghost
{
    public Inky(Point start) : base(start)
    {
        AnimationRow = 6;

        ChaseBehaviour = new ChaseRandom();
        FrightenedBehaviour = new FrightenedWandering();
        EatenBehaviour = new EatenRunningHome();
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board board)
        {
            if (board.Parent is not Game)
                ScatterBehaviour = new ScatterBottomRightCorner(board.Surface.Area);
        }
        base.OnParentChanged(oldParent, newParent);
    }
}