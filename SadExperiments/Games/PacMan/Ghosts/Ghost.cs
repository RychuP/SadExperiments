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
            var prevMode = _mode;
            _mode = value;
            OnModeChanged(prevMode, _mode);
        }
    }

    double SpeedMultiplier
    {
        get
        {
            if (Parent is Board board && board.Parent is Game game)
            {
                return game.Level switch
                {
                    >= Game.MaxDifficultyLevel => 0.95d,
                    >= 5 => 0.95d,
                    >= 2 => 0.85d,
                    _ => 0.75d,
                };
            }
            else
                return 0.95d;
        }
    }

    double FreightSpeedMultiplier
    {
        get
        {
            if (Parent is Board board && board.Parent is Game game)
            {
                return game.Level switch
                {
                    >= Game.MaxDifficultyLevel => 0.95d,
                    >= 5 => 0.60d,
                    >= 2 => 0.55d,
                    _ => 0.50d,
                };
            }
            else
                return 0.95d;
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

    protected virtual void Board_OnFirstStart(object? o, EventArgs e)
    {
        if (o is Board board && board.Parent is Game game)
        {
            // to set proper speed
            Mode = GhostMode.Chase;
        }
        else
            throw new InvalidOperationException("Board is not assigned to a Game.");
    }

    protected virtual void OnModeChanged(GhostMode prevMode, GhostMode newMode)
    {
        switch (newMode)
        {
            case GhostMode.Scatter:
                Speed = Game.SpriteSpeed * SpeedMultiplier;
                break;

            case GhostMode.Chase:
                Speed = Game.SpriteSpeed * SpeedMultiplier;
                break;

            case GhostMode.Frightened:
                Speed = Game.SpriteSpeed * FreightSpeedMultiplier;
                break;

            case GhostMode.Eaten:
                Speed = Game.SpriteSpeed * 2;
                break;
        }

        var args = new GhostModeEventArgs(prevMode, newMode);
        ModeChanged?.Invoke(this, args);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board board)
        {
            // TODO: replace with scatter
            Mode = GhostMode.Chase;

            if (board.Parent is not Game)
                board.FirstStart += Board_OnFirstStart;
        }
        base.OnParentChanged(oldParent, newParent);
    }
    protected override void OnToPositionReached()
    {
        if (Parent is not Board board) return;

        // TODO: remove this line when other ghosts are implemented
        if (this is not Blinky) return;

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

    #region Events
    public event EventHandler<GhostModeEventArgs>? ModeChanged;
    #endregion Events
}

enum GhostMode
{
    Idle, Scatter, Chase, Frightened, Eaten
}

class GhostModeEventArgs : EventArgs
{
    public GhostMode PrevMode { get; init; }
    public GhostMode NewMode { get; init; }
    public GhostModeEventArgs(GhostMode prevMode, GhostMode newMode) =>
        (PrevMode, NewMode) = (prevMode, newMode);
}