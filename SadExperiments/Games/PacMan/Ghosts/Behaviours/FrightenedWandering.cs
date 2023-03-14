using GoRogue.Random;

namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class FrightenedWandering : IFrightenedBehaviour
{
    public Destination Frightened(Board board, Destination prevDestination)
    {
        var randomTurn = Board.GetRandomTurn(prevDestination.Direction);
        var desiredDirection = GlobalRandom.DefaultRNG.NextInt(0, 2) switch
        {
            0 => randomTurn,
            _ => prevDestination.Direction
        };
        return board.GetDestination(prevDestination.Position, desiredDirection, prevDestination.Direction);
    }
}