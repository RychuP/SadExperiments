namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

interface IChaseBehaviour
{
    Destination Chase(Board board, Destination prevDestination);
}