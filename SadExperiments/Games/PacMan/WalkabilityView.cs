using SadRogue.Primitives.GridViews;

namespace SadExperiments.Games.PacMan;

class WalkabilityView : GridViewBase<bool>
{
    readonly Board _board;
    public WalkabilityView(Board board)
    {
        _board = board;
    }
    public override int Width => _board.Surface.Width;
    public override int Height => _board.Surface.Height;
    public override bool this[Point pos] => _board.IsWalkable(pos);
}