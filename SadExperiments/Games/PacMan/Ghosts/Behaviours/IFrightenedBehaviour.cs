namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

interface IFrightenedBehaviour
{
    Destination Frightened(Board board, Destination prevDestination);
}