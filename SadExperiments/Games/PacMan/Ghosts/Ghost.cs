using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

// movement pattern implementation inspired by the article by André Maré https://bit.ly/pacmanpatterns
abstract class Ghost : Sprite
{
    #region Fields
    const int FreightenedAnimationRow = 10;
    const int EatenAnimationRow = 11;
    GhostMode _mode = GhostMode.Idle;
    #endregion Fields

    #region Constructors
    public Ghost(Point start) : base(start)
    {
        AnimationSpeed = 0.25d;
        Speed = 1.9d;
    }
    #endregion Constructors

    #region Properties
    protected IScatterBehaviour? ScatterBehaviour { get; init; } 
    protected IChaseBehaviour? ChaseBehaviour { get; init; }
    protected IFrightenedBehaviour? FrightenedBehaviour { get; init; }
    protected IEatenBehaviour? EatenBehaviour { get; init; }

    public GhostMode Mode
    {
        get => _mode;
        set
        {
            if (_mode == value) return;
            _mode = value;
            OnModeChanged(_mode);
        }
    }
    #endregion Properties

    #region Methods
    protected override int GetAnimationGlyph(int animationColumn, int animationFrame)
    {
        if (Mode == GhostMode.Frightened)
            return FreightenedAnimationRow * 4 + animationFrame;
        else if (Mode == GhostMode.Eaten)
            return EatenAnimationRow * 4 + animationColumn;
        else
            return base.GetAnimationGlyph(animationColumn, animationFrame);
    }

    protected virtual void OnModeChanged(GhostMode newMode)
    {
        switch (newMode)
        {
            case GhostMode.Chase:
                Speed = Game.SpriteSpeed * 0.95d;
                break;

            case GhostMode.Frightened:
                Speed /= 2;
                break;

            case GhostMode.Eaten:
                Speed = Game.SpriteSpeed * 2;
                break;
        }
    }

    protected override void OnToPositionReached()
    {
        if (Parent is not Board board) return;

        // keep this here (portal check)
        base.OnToPositionReached();

        Destination? destination = Mode switch
        {
            GhostMode.Scatter => ScatterBehaviour?.Scatter(board, FromPosition, Direction),
            GhostMode.Chase => ChaseBehaviour?.Chase(board, FromPosition, Direction),
            GhostMode.Frightened => FrightenedBehaviour?.Frightened(board, FromPosition, Direction),
            GhostMode.Eaten => EatenBehaviour?.RunBackHome(board, FromPosition, Direction),
            _ => null
        };

        if (destination != null)
        {
            // ghost reached the center of the house in eaten mode
            if (destination.Position == board.GhostHouse.CenterPosition && destination.Direction == Direction.None)
            {
                if (Mode == GhostMode.Eaten)
                {
                    // set coordinates back to the entrance
                    destination = new Destination(board.GhostHouse.EntrancePosition, Direction.Up);
                    Mode = GhostMode.Chase;
                }
                else
                    throw new ArgumentException("Special case of destination is reserved for eaten behaviour.");
            }

            Direction = destination.Direction;
            ToPosition = destination.Position;
        }
    }
    #endregion Methods
}

enum GhostMode
{
    Idle, Scatter, Chase, Frightened, Eaten
}