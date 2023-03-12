using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

class Clyde : Ghost
{
    public Clyde(Point start) : base(start)
    {
        AnimationRow = 8;

        ScatterBehaviour = new ScatterBottomLeftCorner();
        ChaseBehaviour = new ChasePatrol();
        FrightenedBehaviour = new FrightenedWandering();
        EatenBehaviour = new EatenRunningHome();
    }
}