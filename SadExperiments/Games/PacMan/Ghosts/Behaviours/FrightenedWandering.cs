using GoRogue.Random;

namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class FrightenedWandering : IFrightenedBehaviour
{
    public Destination Frightened(Board board, Point position, Direction direction)
    {
        var randomTurn = Board.GetRandomTurn(direction);
        var desiredDirection = GlobalRandom.DefaultRNG.NextInt(0, 2) switch
        {
            0 => randomTurn,
            _ => direction
        };
        return board.GetDestination(position, desiredDirection, direction);
    }
}