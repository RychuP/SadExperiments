namespace SadExperiments.Games.PacMan;

class Player : Sprite
{
    public Player()
    {
        AnimationRow = 0;
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        base.OnParentChanged(oldParent, newParent);
        if (Parent is Board board)
        {
            Direction = Direction.Left;
            NextDirection = Direction.None;
            ToPosition = board.GetNextPosition(FromPosition, Direction);
        }
    }

    protected override void OnToPositionReached()
    {
        if (Parent is Board board)
        {
            // check premove first
            if (NextDirection != Direction.None)
            {
                // check if the direction is leading to a valid cell
                var position = board.GetNextPosition(FromPosition, NextDirection);
                if (position != FromPosition)
                {
                    Direction = NextDirection;
                    NextDirection = Direction.None;
                    ToPosition = position;
                }

                // try to continue in the prev direction
                else
                {
                    position = board.GetNextPosition(FromPosition, Direction);
                    if (position != FromPosition)
                        ToPosition = position;

                    // nothing works... just stop
                    else
                    {
                        Direction = Direction.None;
                        NextDirection = Direction.None;
                    }
                }
            }

            // try to continue in the prev direction
            else if (Direction != Direction.None)
            {
                var position = board.GetNextPosition(FromPosition, Direction);
                if (position != FromPosition)
                    ToPosition = position;
                else
                    Direction = Direction.None;
            }
        }

        base.OnToPositionReached();
    }
}