using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Inky : Ghost
{
    public Inky(Board board, Point start) : base(board, start)
    {
        AnimationRow = 6;
        ChaseBehaviour = new ChaseDiagonal();
        ScatterBehaviour = new ScatterBottomRightCorner(board.Surface.Area, this);
    }
}