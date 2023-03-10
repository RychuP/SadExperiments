using SadExperiments.Games.Tetris;

namespace SadExperiments.Games.PacMan;

class Player : Sprite
{
    const int DeathAnimStartIndex = 48;
    const int DeathAnimEndIndex = 63;
    readonly TimeSpan _animationSpeed = TimeSpan.FromSeconds(13 / 60d);
    TimeSpan _timeElapsed = TimeSpan.Zero;
    int _deathAnimCurrentIndex = 0;

    public Player(Point start) : base(start)
    {
        AnimationRow = 0;
        Speed = 2d;
    }

    public bool IsDead { get; private set; }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        // prepare to start
        if (newParent is Board)
        {
            IsDead = false;
            Direction = Direction.Left;
            NextDirection = Direction.None;
        }
        base.OnParentChanged(oldParent, newParent);
    }

    protected override void OnToPositionReached()
    {
        if (Parent is not Board board) return;

        // check if there is a consumable at the current location
        IEdible? edible = board.GetConsumable(FromPosition);
        if (edible is Dot dot)
        {
            board.RemoveDot(dot);
            Sounds.Munch.Play();
        }
        else
            Sounds.Munch.Stop();

        // check if the next direction is set
        if (NextDirection != Direction.None)
        {
            // try going in the next direction
            if (TrySetToPosition(NextDirection))
                NextDirection = Direction.None;

            // try to continue in the prev direction
            else if (!TrySetToPosition(Direction))
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
            if (!TrySetToPosition(Direction))
                Direction = Direction.None;
        }

        base.OnToPositionReached();
    }

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

    protected virtual void OnDeathAnimationFinished()
    {
        DeathAnimationFinished?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? DeathAnimationFinished;
}