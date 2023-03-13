namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ScatterTopRightCorner : ScatterBase
{
    public ScatterTopRightCorner(Rectangle boardArea)
    {
        var bisection = boardArea.BisectHorizontally();
        Area = bisection.Rect1.BisectVertically().Rect2;
    }
}