namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

interface IFrightenedBehaviour
{
    Destination Frightened(Board board, Point ghostPosition, Direction ghostDirection);
}