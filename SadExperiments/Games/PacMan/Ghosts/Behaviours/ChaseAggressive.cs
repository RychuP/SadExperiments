namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

class ChaseAggressive : ChaseBaseBehaviour
{
    public override Destination Chase(Board board, Point position, Direction direction)
    {
        var destination = board.GetPlayerPosition();
        return Navigate(board, position, direction, destination);
    }
}