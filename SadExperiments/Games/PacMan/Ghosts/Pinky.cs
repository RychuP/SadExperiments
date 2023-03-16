using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Pinky : Ghost
{
    public Pinky(Board board, Point start) : base(board, start)
    {
        AnimationRow = 4;
        ChaseBehaviour = new ChaseAmbush();
        ScatterBehaviour = new ScatterTopLeftCorner(board.Surface.Area, this);
    }
}