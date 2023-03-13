using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Clyde : Ghost
{
    public Clyde(Point start) : base(start)
    {
        AnimationRow = 8;

        ChaseBehaviour = new ChasePatrol();
        FrightenedBehaviour = new FrightenedWandering();
        EatenBehaviour = new EatenRunningHome();
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board board)
        {
            if (board.Parent is not Game)
                ScatterBehaviour = new ScatterBottomLeftCorner(board.Surface.Area);
        }
        base.OnParentChanged(oldParent, newParent);
    }
}