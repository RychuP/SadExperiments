namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class IdleWaitingBehaviour : IIdleBehaviour
{
    public Destination Idle() =>
        Destination.None;
}