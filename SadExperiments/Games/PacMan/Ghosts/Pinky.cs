using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Pinky : Ghost
{
    public Pinky(Point start) : base(start)
    {
        AnimationRow = 4;

        ScatterBehaviour = new ScatterTopLeftCorner();
        ChaseBehaviour = new ChaseAmbush();
        FrightenedBehaviour = new FrightenedWandering();
        EatenBehaviour = new EatenRunningHome();
    }
}