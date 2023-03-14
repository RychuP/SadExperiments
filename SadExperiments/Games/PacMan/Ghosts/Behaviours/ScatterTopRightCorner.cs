namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ScatterTopRightCorner : ScatterBase
{
    public ScatterTopRightCorner(Rectangle boardArea, Ghost host) : base(host)
    {
        var bisection = boardArea.BisectHorizontally();
        Area = bisection.Rect1.BisectVertically().Rect2;
    }
}