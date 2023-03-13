namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ScatterBottomRightCorner : ScatterBase
{
    public ScatterBottomRightCorner(Rectangle boardArea)
    {
        var bisection = boardArea.BisectHorizontally();
        Area = bisection.Rect2.BisectVertically().Rect2;
    }
}