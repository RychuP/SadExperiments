using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Blinky : Ghost
{
    public Blinky(Point start) : base(start)
    {
        AnimationRow = 2;

        ScatterBehaviour = new ScatterTopRightCorner();
        ChaseBehaviour = new ChaseAggressive();
        FrightenedBehaviour = new FrightenedWandering();
        EatenBehaviour = new EatenRunningHome();
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board)
        {
            Direction = Board.GetRandomTurn(Direction.Down);
        }
        base.OnParentChanged(oldParent, newParent);
    }
}