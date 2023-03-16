namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

interface IScatterBehaviour
{
    Destination Scatter(Board board, Point position, Direction direction);
}