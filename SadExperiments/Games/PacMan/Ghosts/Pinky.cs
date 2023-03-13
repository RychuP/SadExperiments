using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Pinky : Ghost
{
    public Pinky(Point start) : base(start)
    {
        AnimationRow = 4;

        //ChaseBehaviour = new ChaseAmbush();
        ChaseBehaviour = new ChaseAggressive();
        EatenBehaviour = new EatenRunningHome();
        AwakeBehaviour = new WakenUpBehaviour();
        FrightenedBehaviour = new FrightenedWandering();
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        Direction = Direction.None;
        base.OnParentChanged(oldParent, newParent);

        // create scatter behaviour once before the board is assigned to a game
        if (newParent is Board board)
        {
            if (board.Parent is Game)
            {
                Mode = GhostMode.Awake;
                FromPosition = Start;
                OnToPositionReached();
            }
            else
                ScatterBehaviour = new ScatterTopLeftCorner(board.Surface.Area);
        }
    }

    protected override void Board_OnFirstStart(object? o, EventArgs e)
    {
        base.Board_OnFirstStart(o, e);
        if (o is Board board && board.Parent is Game)
        {
            Mode = GhostMode.Awake;
            FromPosition = Start;
            OnToPositionReached();
        }
    }
}