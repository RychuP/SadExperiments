namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ScatterTopRightCorner : ScatterBaseBehaviour
{
    public ScatterTopRightCorner(Rectangle boardArea, Ghost host) : base(host)
    {
        var bisection = boardArea.BisectHorizontally();
        Area = bisection.Rect1.BisectVertically().Rect2;
    }
}