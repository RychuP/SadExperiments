namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ScatterTopLeftCorner : ScatterBase
{
    public ScatterTopLeftCorner(Rectangle boardArea)
    {
        var bisection = boardArea.BisectHorizontally();
        Area = bisection.Rect1.BisectVertically().Rect1;
    }
}