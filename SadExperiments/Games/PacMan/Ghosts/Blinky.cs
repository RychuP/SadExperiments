using GoRogue.Random;
using Newtonsoft.Json.Linq;

namespace SadExperiments.Games.PacMan.Ghosts;

class Blinky : Ghost
{
    public Blinky(Point start) : base(start)
    {
        AnimationRow = 2;
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board)
        {
            // prepare to start
            Direction = GlobalRandom.DefaultRNG.NextInt(0, 2) switch
            {
                0 => Direction.Left,
                _ => Direction.Right
            };
        }
        base.OnParentChanged(oldParent, newParent);
    }

    protected override void OnToPositionReached()
    {
        if (Parent is not Board board) return;

        // leave this here (check for portals)
        base.OnToPositionReached();

        var nextPosition = board.GetNextPositionToPlayer(FromPosition);
        var direction = Direction.GetDirection(FromPosition, nextPosition);

        // check astar direction
        if (direction != Direction.Inverse() && direction != Direction.None)
        {
            Direction = direction;
            ToPosition = nextPosition;
        }
        // find own direction
        else
        {
            // get direction to player
            direction = Direction.GetDirection(FromPosition, board.Player.ToPosition);
            direction = (Direction == Direction.Left || Direction == Direction.Right) ?
                direction.DeltaY switch
                {
                    -1 => Direction.Up,
                    1 => Direction.Down,
                    _ => RandomTurn
                } :
                direction.DeltaX switch
                {
                    -1 => Direction.Right,
                    1 => Direction.Left,
                    _ => RandomTurn
                };

            // try going in the new direction
            if (!TrySetToPosition(direction))
            {
                // try going in the current direction
                if (!TrySetToPosition(Direction))
                {
                    // try going in the remaining direction
                    if (!TrySetToPosition(direction.Inverse()))
                    {
                        
                        // dead end without a valid turn? this shouldn't happen...
                        throw new InvalidOperationException("Can't find a valid direction for the ghost.");
                    }
                }
            }
        }
    }
}