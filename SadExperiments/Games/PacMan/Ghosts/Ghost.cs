using GoRogue.Random;
using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

// movement pattern implementation inspired by the article by André Maré https://bit.ly/pacmanpatterns
abstract class Ghost : Sprite
{
    #region Fields
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
    public static Direction GetRandomTurn(Direction direction)
    {
        return GlobalRandom.DefaultRNG.NextInt(0, 2) switch
        {
            0 => direction + 2,
            _ => direction - 2
        };
    }

    protected virtual void OnModeChanged(GhostMode newMode) { }

    protected override void OnToPositionReached()
    {
        if (Parent is not Board board) return;

        base.OnToPositionReached();

        Destination? destination = Mode switch
        {
            GhostMode.Scatter => ScatterBehaviour?.Scatter(board, FromPosition, Direction),
            GhostMode.Chase => ChaseBehaviour?.Chase(board, FromPosition, Direction),
            GhostMode.Frightened => FrightenedBehaviour?.Frightened(board, FromPosition, Direction),
            _ => null
        };

        if (destination != null)
        {
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