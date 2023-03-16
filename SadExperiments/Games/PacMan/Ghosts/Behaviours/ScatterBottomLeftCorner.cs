namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ScatterBottomLeftCorner : ScatterBaseBehaviour
{
    public ScatterBottomLeftCorner(Rectangle boardArea, Ghost host) : base(host)
    {
        var bisection = boardArea.BisectHorizontally();
        Area = bisection.Rect2.BisectVertically().Rect1;
    }
}