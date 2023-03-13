using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Blinky : Ghost
{
    public Blinky(Point start) : base(start)
    {
        AnimationRow = 2;

        ChaseBehaviour = new ChaseAggressive();
        EatenBehaviour = new EatenRunningHome();
        AwakeBehaviour = new WakenUpBehaviour();
        FrightenedBehaviour = new FrightenedWandering();
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        // create scatter behaviour once before the board is assigned to a game
        if (newParent is Board board)
        {
            // leave direction setting here (sprite will set destination)
            Direction = Board.GetRandomTurn(Direction.Down);
            Mode = GhostMode.Scatter;

            if (board.Parent is not Game)
                ScatterBehaviour = new ScatterTopRightCorner(board.Surface.Area);
        }
        base.OnParentChanged(oldParent, newParent);
    }

    protected override void Board_OnFirstStart(object? o, EventArgs e)
    {
        base.Board_OnFirstStart(o, e);
        if (o is Board board && board.Parent is Game)
        {
            // leave mode setting here (sets proper speed taking into account the game level)
            Mode = GhostMode.Scatter;
        }
    }
}