namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ScatterBottomRightCorner : ScatterBaseBehaviour
{
    public ScatterBottomRightCorner(Rectangle boardArea, Ghost host) : base(host)
    {
        var bisection = boardArea.BisectHorizontally();
        Area = bisection.Rect2.BisectVertically().Rect2;
    }
}