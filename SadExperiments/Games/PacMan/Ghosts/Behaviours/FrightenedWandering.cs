using GoRogue.Random;

namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class FrightenedWandering : IFrightenedBehaviour
{
    public Destination Frightened(Board board, Point ghostPosition, Direction ghostDirection)
    {
        var randomTurn = Board.GetRandomTurn(ghostDirection);
        var desiredDirection = GlobalRandom.DefaultRNG.NextInt(0, 2) switch
        {
            0 => randomTurn,
            _ => ghostDirection
        };
        return board.GetDestination(ghostPosition, desiredDirection, ghostDirection);
    }
}