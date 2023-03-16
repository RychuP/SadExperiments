namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

interface IChaseBehaviour
{
    Destination Chase(Board board, Point position, Direction direction);
}