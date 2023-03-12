using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Inky : Ghost
{
    public Inky(Point start) : base(start)
    {
        AnimationRow = 6;

        ScatterBehaviour = new ScatterBottomRightCorner();
        ChaseBehaviour = new ChaseRandom();
        FrightenedBehaviour = new FrightenedWandering();
        EatenBehaviour = new EatenRunningHome();
    }
}