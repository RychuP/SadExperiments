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
    public Ghost(Board board, Point start) : base(board, start)
    {
        AnimationSpeed = 0.25d;
        EatenBehaviour = new EatenRunningHome();
        AwakeBehaviour = new WakenUpBehaviour();
        IdleBehaviour = new IdleWaitingBehaviour();
        FrightenedBehaviour = new FrightenedWandering();
    }
    #endregion Constructors

    #region Properties
    protected IScatterBehaviour? ScatterBehaviour { get; set; } 
    protected IChaseBehaviour? ChaseBehaviour { get; init; }
    protected IFrightenedBehaviour? FrightenedBehaviour { get; init; }
    protected IEatenBehaviour? EatenBehaviour { get; init; }
    protected IIdleBehaviour? IdleBehaviour { get; init; }
    protected IAwakeBehaviour? AwakeBehaviour { get; init; }

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
            return Board.Game.Level switch
            {
                >= Game.MaxDifficultyLevel => 0.95d,
                >= 5 => 0.95d,
                >= 2 => 0.85d,
                _ => 0.75d,
            };
        }
    }

    double FreightSpeedMultiplier
    {
        get
        {
            return Board.Game.Level switch
            {
                >= Game.MaxDifficultyLevel => 0.95d,
                >= 5 => 0.60d,
                >= 2 => 0.55d,
                _ => 0.50d,
            };
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

            case GhostMode.Awake:
                Speed = Game.SpriteSpeed * SpeedMultiplier;
                break;
        }

        if (prevMode == GhostMode.Idle && newMode == GhostMode.Awake && AwakeBehaviour is not null)
            Destination = AwakeBehaviour.LeaveHouse(Board, Start);

        var args = new GhostModeEventArgs(prevMode, newMode);
        ModeChanged?.Invoke(this, args);
    }

    protected override void OnDestinationReached(Departure departure, Destination destination)
    {
        if (Mode == GhostMode.Idle) return;

        // check if the position is a portal
        if (Board.IsPortal(destination.Position, out Portal? portal))
        {
            if (portal == null)
                throw new ArgumentException("Portal needs to have a valid matching destination.");

            // teleport to the matching portal on the other side of the board
            Departure = new(portal.Position);

            // set destination in previous direction
            if (!TrySetDestination(Destination.Direction))
                throw new InvalidOperationException("Portal has got no walkable tile in the exit direction.");
        }

        // position is not a portal
        else
        {
            Destination? nullable = Mode switch
            {
                GhostMode.Scatter => ScatterBehaviour?.Scatter(Board, destination.Position, destination.Direction),
                GhostMode.Chase => ChaseBehaviour?.Chase(Board, destination.Position, destination.Direction),
                GhostMode.Frightened => FrightenedBehaviour?.Frightened(Board, destination.Position, destination.Direction),
                GhostMode.Eaten => EatenBehaviour?.RunBackHome(Board, destination.Position),
                GhostMode.Awake => AwakeBehaviour?.LeaveHouse(Board, destination.Position),
                _ => IdleBehaviour?.Idle()
            };

            if (nullable is Destination newDestination && newDestination != Destination.None)
            {
                // ghost is in the center of the house and leaving
                if (newDestination.Position == Board.GhostHouse.EntrancePosition && newDestination.Direction == Direction.Up)
                    Mode = Board.GhostHouse.CurrentMode;

                Destination = newDestination;
            }
        }

        base.OnDestinationReached(departure, destination);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        AnimationColumn = 0;
        base.OnParentChanged(oldParent, newParent);

        if (newParent is Board)
            Mode = GhostMode.Idle;
    }
    #endregion Methods

    #region Events
    public event EventHandler<GhostModeEventArgs>? ModeChanged;
    #endregion Events
}

enum GhostMode
{
    Idle, Awake, Scatter, Chase, Frightened, Eaten
}