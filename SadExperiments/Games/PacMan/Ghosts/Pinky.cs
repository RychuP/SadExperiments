using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Pinky : Ghost
{
    public Pinky(Point start) : base(start)
    {
        AnimationRow = 4;

        ChaseBehaviour = new ChaseAmbush();
        FrightenedBehaviour = new FrightenedWandering();
        EatenBehaviour = new EatenRunningHome();
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board board)
        {
            if (board.Parent is not Game)
                ScatterBehaviour = new ScatterTopLeftCorner(board.Surface.Area);
        }
        base.OnParentChanged(oldParent, newParent);
    }
}