using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Blinky : Ghost
{
    public Blinky(Point start) : base(start)
    {
        AnimationRow = 2;

        ChaseBehaviour = new ChaseAggressive();
        FrightenedBehaviour = new FrightenedWandering();
        EatenBehaviour = new EatenRunningHome();
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board board)
        {
            Direction = Board.GetRandomTurn(Direction.Down);

            if (board.Parent is not Game)
                ScatterBehaviour = new ScatterTopRightCorner(board.Surface.Area);
        }
        base.OnParentChanged(oldParent, newParent);
    }
}