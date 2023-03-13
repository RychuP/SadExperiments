using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Clyde : Ghost
{
    public Clyde(Point start) : base(start)
    {
        AnimationRow = 8;

        //ChaseBehaviour = new ChasePatrol();
        ChaseBehaviour = new ChaseAggressive();
        EatenBehaviour = new EatenRunningHome();
        AwakeBehaviour = new WakenUpBehaviour();
        FrightenedBehaviour = new FrightenedWandering();
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        // create scatter behaviour once before the board is assigned to a game
        if (newParent is Board board && board.Parent is not Game)
            ScatterBehaviour = new ScatterBottomLeftCorner(board.Surface.Area);
        base.OnParentChanged(oldParent, newParent);
    }
}