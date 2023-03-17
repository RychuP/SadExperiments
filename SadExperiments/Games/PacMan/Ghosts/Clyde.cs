using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Clyde : Ghost
{
    public Clyde(Board board, Point start) : base(board, start)
    {
        AnimationRow = 8;
        ChaseBehaviour = new ChaseRandom(this);
        ScatterBehaviour = new ScatterBottomLeftCorner(board.Surface.Area, this);
    }
}