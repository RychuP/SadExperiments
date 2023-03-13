namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ScatterBottomLeftCorner : ScatterBase
{
    public ScatterBottomLeftCorner(Rectangle boardArea)
    {
        var bisection = boardArea.BisectHorizontally();
        Area = bisection.Rect2.BisectVertically().Rect1;
    }
}