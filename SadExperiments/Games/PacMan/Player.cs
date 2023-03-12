namespace SadExperiments.Games.PacMan;

class Player : Sprite
{
    #region Fields
    const int DeathAnimStartIndex = 48;
    const int DeathAnimEndIndex = 63;
    readonly TimeSpan _animationSpeed = TimeSpan.FromSeconds(13 / 60d);
    TimeSpan _timeElapsed = TimeSpan.Zero;
    int _deathAnimCurrentIndex = 0;
    #endregion Fields

    #region Constructors
    public Player(Point start) : base(start)
    {
        AnimationRow = 0;
    }
    #endregion Constructors

    #region Properties
    public bool IsDead { get; private set; }
    #endregion Properties

    #region Methods
    public override void Update(TimeSpan delta)
    {
        if (!IsDead)
            // do a normal animation update
            base.Update(delta);
        else
        {
            // play death animation
            _timeElapsed += delta;
            if (_timeElapsed >= _animationSpeed)
            {
                if (_deathAnimCurrentIndex == DeathAnimEndIndex)
                {
                    OnDeathAnimationFinished();
                }
                else
                {
                    _timeElapsed = TimeSpan.Zero;
                    Surface[0].Glyph = ++_deathAnimCurrentIndex;
                    Surface.IsDirty = true;
                }
            }
        }
    }

    public void Die()
    {
        IsDead = true;
        Surface[0].Glyph = _deathAnimCurrentIndex = DeathAnimStartIndex;
        Surface.IsDirty = true;
    }

    void Board_OnGameStart(object? o, EventArgs e)
    {
        if (o is Board board && board.Parent is Game game)
        {
            Speed = Game.SpriteSpeed * game.Level switch
            {
                >= Game.MaxDifficultyLevel => 0.9d,
                >= 5 => 1d,
                >= 2 => 0.9d,
                _ => 0.8d
            };
        }
        else
            throw new InvalidOperationException("Board is not assigned to a Game.");
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        // prepare to start
        if (newParent is Board board)
        {
            IsDead = false;
            Direction = Direction.Left;
            NextDirection = Direction.None;

            if (board.Parent is not Game)
                board.FirstStart += Board_OnGameStart;
        }
        base.OnParentChanged(oldParent, newParent);
    }

    protected override void OnToPositionReached()
    {
        if (Parent is not Board board) return;

        // leave this here (check for portals)
        base.OnToPositionReached();

        // check if there is a consumable at the current location
        IEdible? edible = board.GetConsumable(FromPosition);
        if (edible is Dot dot)
            board.RemoveDot(dot);
        else
            Sounds.MunchDot.Stop();

        // check if the next direction is set
        if (NextDirection != Direction.None)
        {
            // try going in the next direction
            if (TrySetDestination(NextDirection))
                NextDirection = Direction.None;

            // try to continue in the prev direction
            else if (!TrySetDestination(Direction))
            {
                // wrong inputs... just stop
                Direction = Direction.None;
                NextDirection = Direction.None;
            }
        }

        // next direction is not set, so try continuing in the prev direction
        else if (Direction != Direction.None)
        {
            // if it's not valid, just stop
            if (!TrySetDestination(Direction))
                Direction = Direction.None;
        }
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