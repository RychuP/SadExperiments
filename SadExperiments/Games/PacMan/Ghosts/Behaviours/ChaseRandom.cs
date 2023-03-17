namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ChaseRandom : ChaseBaseBehaviour
{
    const int PatrolRadius = 4;

    public ChaseRandom(Ghost host)
    {
        host.ModeChanged += Ghost_OnModeChanged;
    }

    public override Destination Chase(Board board, Point position, Direction direction)
    {
        if (ToPosition == Point.None || ToPosition == position)
        {
            var playerPos = board.GetPlayerPosition();
            Rectangle playerArea = new(playerPos, PatrolRadius, PatrolRadius);
            ToPosition = GetRandValidPosInArea(board, playerArea);
        }

        return Navigate(board, position, direction, ToPosition);
    }
}