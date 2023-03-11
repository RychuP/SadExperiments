namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ChaseAggressive : IChaseBehaviour
{
    public Destination Chase(Board board, Point ghostPosition, Direction ghostDirection)
    {
        var positionLeadingToPlayer = board.GetPositionToPlayer(ghostPosition);
        var direction = Direction.GetDirection(ghostPosition, positionLeadingToPlayer);

        // check astar direction
        if (direction != ghostDirection.Inverse() && direction != Direction.None)
            return new Destination(positionLeadingToPlayer, direction);

        // find own direction
        else
        {
            // get direction to player
            direction = Direction.GetDirection(ghostPosition, board.Player.ToPosition);
            direction = (ghostDirection == Direction.Left || ghostDirection == Direction.Right) ?
                direction.DeltaY switch
                {
                    -1 => Direction.Up,
                    1 => Direction.Down,
                    _ => Ghost.GetRandomTurn(ghostDirection)
                } :
                direction.DeltaX switch
                {
                    -1 => Direction.Right,
                    1 => Direction.Left,
                    _ => Ghost.GetRandomTurn(ghostDirection)
                };

            // try the new direction
            var position = board.GetNextPosition(ghostPosition, direction);
            if (position != ghostPosition)
                return new Destination(position, direction);

            else {
                // try the current direction
                position = board.GetNextPosition(ghostPosition, ghostDirection);
                if (position != ghostPosition)
                    return new Destination(position, ghostDirection);

                else {
                    // try the remaining direction
                    direction = direction.Inverse();
                    position = board.GetNextPosition(ghostPosition, direction);
                    if (position != ghostPosition)
                        return new Destination(position, direction);
                    
                    else
                        // dead end without a valid turn? this shouldn't happen...
                        throw new InvalidOperationException("Can't find a valid direction for the ghost.");
                }
            }
        }
    }
}