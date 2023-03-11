namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

interface IScatterBehaviour
{
    Destination Scatter(Board board, Point ghostPosition, Direction ghostDirection);
}