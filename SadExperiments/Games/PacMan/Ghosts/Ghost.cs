using SadConsole.Components;
using SadExperiments.Games.PacMan.Ghosts.Behaviours;

namespace SadExperiments.Games.PacMan.Ghosts;

// movement pattern implementation inspired by the article by André Maré https://bit.ly/pacmanpatterns
abstract class Ghost : Sprite
{
    #region Fields
    // starting mode
    GhostMode _mode = GhostMode.Idle;

    // animation rows
    const int FreightenedAnimationRow = 10;
    const int EatenAnimationRow = 11;
    
    // freightened mode
    bool _whiteFlash = false;
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
    // checks if the ghost overlaps any another ghost going in the same direction
    bool IsOverlapping()
    {
        foreach (var ghost in Board.GhostHouse.Ghosts)
            if (ghost != this && ghost.Destination.Direction != Destination.Direction && ghost.HitBox.Intersects(HitBox))
                return true;
        return false;
    }

    public bool IsInGhostHouse() =>
        Board.GhostHouse.PixelArea.Intersects(HitBox);

    public void ShowValue(int value)
    {
        Surface[0].Glyph = value switch
        {
            1600 => 63,
            800 => 62,
            400 => 61,
            _ => 60
        };
        Surface.IsDirty = true;
    }

    protected override int GetAnimationGlyph(int animationColumn, int animationFrame)
    {
        if (Mode == GhostMode.Frightened)
        {
            // in transiton period alternate between the blue and white animation
            if (Board.GhostHouse.PowerDotTransition)
            {
                if (_whiteFlash)
                {
                    if (animationFrame == 1)
                        _whiteFlash = false;
                    return FreightenedAnimationRow * 4 + animationFrame + 2;
                }
                else
                {
                    if (animationFrame == 1)
                        _whiteFlash = true;
                    return FreightenedAnimationRow * 4 + animationFrame;
                }
            }

            // normal blue mode without flashes
            else
                return FreightenedAnimationRow * 4 + animationFrame;
        }
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

        // leave house on awake
        if (prevMode == GhostMode.Idle && newMode == GhostMode.Awake && AwakeBehaviour is not null)
            Destination = AwakeBehaviour.LeaveHouse(Board, Start);

        // reset the white flash variable
        else if (newMode == GhostMode.Frightened)
            _whiteFlash = true;

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
            // if ghost is overlapping another one just do a random turn left or right
            if (this is not Blinky && IsOverlapping() && !IsInGhostHouse())
            {
                var desiredDirection = Board.GetRandomTurn(destination.Direction);
                var randTurnDest = Board.GetDestination(destination.Position, desiredDirection, destination.Direction);
                
                // check if the turn produced a valid destination, otherwise try an opposite turn
                if (randTurnDest.Direction == desiredDirection)
                    Destination = randTurnDest;
                else
                {
                    desiredDirection = desiredDirection.Inverse();
                    Destination = Board.GetDestination(destination.Position, desiredDirection, destination.Direction);
                }
            }

            // get destination from behaviour
            else
            {
                Destination? destFromBehaviour = Mode switch
                {
                    GhostMode.Scatter => ScatterBehaviour?.Scatter(Board, destination.Position, destination.Direction),
                    GhostMode.Chase => ChaseBehaviour?.Chase(Board, destination.Position, destination.Direction),
                    GhostMode.Frightened => FrightenedBehaviour?.Frightened(Board, destination.Position, destination.Direction),
                    GhostMode.Eaten => EatenBehaviour?.RunBackHome(Board, destination.Position),
                    GhostMode.Awake => AwakeBehaviour?.LeaveHouse(Board, destination.Position),
                    _ => IdleBehaviour?.Idle()
                };

                if (destFromBehaviour is Destination newDestination && newDestination != Destination.None)
                {
                    // ghost is in the center of the house and leaving
                    if (newDestination.Position == Board.GhostHouse.EntrancePosition && newDestination.Direction == Direction.Up)
                        Mode = Board.GhostHouse.CurrentMode;

                    Destination = newDestination;
                }
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