using SadExperiments.Games.Tetris;

namespace SadExperiments.Games.PacMan;

class Player : Sprite
{
    #region Fields
    // animation data
    const int DeathAnimStartIndex = 48;
    const int DeathAnimEndIndex = 59;
    readonly TimeSpan _animationSpeed = TimeSpan.FromSeconds(0.22d);
    TimeSpan _timeElapsed = TimeSpan.Zero;
    int _deathAnimCurrentIndex = 0;

    // cached premove for the next tile/junction
    Direction _nextDirection = Direction.None;
    #endregion Fields

    #region Constructors
    public Player(Board board, Point start) : base(board, start)
    {
        AnimationRow = 0;
        AnimationColumn = 1;
        Speed = Game.SpriteSpeed * board.Game.Level switch
        {
            >= Game.MaxDifficultyLevel => 0.9d,
            >= 5 => 1d,
            >= 2 => 0.9d,
            _ => 0.8d
        };
        Parent = board;
    }
    #endregion Constructors

    #region Properties
    public bool IsDead { get; private set; }

    public Direction NextDirection
    {
        get => _nextDirection;
        set
        {
            if (_nextDirection == value) return;

            var prevNextDirection = _nextDirection;
            _nextDirection = value;
            OnNextDirectionChanged(prevNextDirection, _nextDirection);
        }
    }
    #endregion Properties

    #region Methods
    public void PlayDeathAnimation(TimeSpan delta)
    {
        _timeElapsed += delta;
        if (_timeElapsed >= _animationSpeed && _deathAnimCurrentIndex <= DeathAnimEndIndex)
        {
            if (_deathAnimCurrentIndex == DeathAnimEndIndex)
            {
                _deathAnimCurrentIndex++;
                OnDeathAnimationFinished();
            }
            else if (_deathAnimCurrentIndex < DeathAnimEndIndex)
            {
                _timeElapsed = TimeSpan.Zero;
                Surface[0].Glyph = ++_deathAnimCurrentIndex;
                Surface.IsDirty = true;
            }
        }
    }

    public void Die()
    {
        IsDead = true;
        Surface[0].Glyph = _deathAnimCurrentIndex = DeathAnimStartIndex;
        Surface.IsDirty = true;
    }

    // used only with the player sprite
    virtual protected void OnNextDirectionChanged(Direction prevNextDirection, Direction newNextDirection)
    {
        if (Parent != Board) return;

        if (newNextDirection != Direction.None)
        {
            if (Destination != Destination.None)
            {
                // check for reversing current direction
                if (newNextDirection == Destination.Direction.Inverse())
                {
                    NextDirection = Direction.None;
                    var destination = Departure.Position;
                    Departure = new(Destination.Position);
                    Destination = new(destination, newNextDirection);
                }
            }
            // check for stopped situations
            else
            {
                var position = Board.GetNextPosition(Departure.Position, newNextDirection);
                if (position != Departure.Position)
                {
                    NextDirection = Direction.None;
                    Destination = new(position, newNextDirection);
                }
            }
        }
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        AnimationColumn = 1;
        base.OnParentChanged(oldParent, newParent);

        // prepare to start
        if (newParent is Board)
        {
            IsDead = false;
            NextDirection = Direction.None;
            if (!TrySetDestination(Direction.Left))
                throw new InvalidOperationException("Invalid start location for the player.");
        }
    }

    protected override void OnDestinationReached(Departure departure, Destination destination)
    {
        // check if the position is a portal
        if (Board.IsPortal(destination.Position, out Portal? portal))
        {
            if (portal == null)
                throw new ArgumentException("Portal needs to have a valid matching destination.");

            // teleport to the matching portal on the other side of the board
            Departure = new(portal.Position);

            // set destination in previous direction
            if (!TrySetDestination(destination.Direction))
                throw new InvalidOperationException("Portal has got no walkable tile in the exit direction.");
        }

        // position is not a portal
        else 
        {
            // check if there is a consumable at the current location
            IEdible? edible = Board.GetConsumable(destination.Position);
            if (edible is Dot dot)
                Board.RemoveDot(dot);
            else
                Sounds.MunchDot.Stop();

            // check if the next direction is set
            if (NextDirection != Direction.None)
            {
                // try going in the next direction
                if (TrySetDestination(NextDirection))
                    NextDirection = Direction.None;

                // try to continue in the prev direction
                else if (!TrySetDestination(Destination.Direction))
                {
                    // wrong inputs... just stop
                    Destination = Destination.None;
                    NextDirection = Direction.None;
                }
            }

            // next direction is not set, so try continuing in the prev direction
            else if (Destination.Direction != Direction.None)
            {
                // if it's not valid, just stop
                if (!TrySetDestination(Destination.Direction))
                    Destination = Destination.None;
            }

            if (Destination == Destination.None 
                && Sounds.MunchDot.State != Microsoft.Xna.Framework.Audio.SoundState.Stopped)
                    Sounds.MunchDot.Stop();
                
        }

        base.OnDestinationReached(departure, destination);
    }

    protected virtual void OnDeathAnimationFinished()
    {
        DeathAnimationFinished?.Invoke(this, EventArgs.Empty);
    }
    #endregion Methods

    #region Events
    public event EventHandler? DeathAnimationFinished;
    #endregion Events
}