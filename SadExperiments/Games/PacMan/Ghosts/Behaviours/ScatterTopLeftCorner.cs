namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ScatterTopLeftCorner : ScatterBaseBehaviour
{
    public ScatterTopLeftCorner(Rectangle boardArea, Ghost host) : base(host)
    {
        var bisection = boardArea.BisectHorizontally();
        Area = bisection.Rect1.BisectVertically().Rect1;
    }
}