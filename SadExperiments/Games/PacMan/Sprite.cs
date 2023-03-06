namespace SadExperiments.Games.PacMan;

abstract class Sprite : ScreenSurface
{
    static readonly Point DefaultFontSize = Game.DefaultFontSize + new Point(3, 3) * Game.FontSizeMultiplier;
    Direction _direction = Direction.None;

    // cached premove for the next tile/junction
    Direction _nextDirection = Direction.None;

    // movement data
    Point _fromPosition = Point.None;
    Point _toPosition = Point.None;
    Point _currentPosition = Point.None;
    double _distanceTravelled = 0;

    // sprites occupy more space than a board tile (their position is offsetted a few pixels)
    readonly Point _positionOffset;

    // two frame animation data
    readonly TimeSpan _animationFreq = TimeSpan.FromSeconds((double)8 / 60);
    TimeSpan _timeElapsed = TimeSpan.Zero;
    int _currentAnimFrame = 0;
    int _animationColumn = 0;
    int _firstFrameIndex = 0;

    public Sprite() : base(1, 1)
    {
        Font = Fonts.Sprites;
        UsePixelPositioning = true;

        // calculate sprite position offset
        int offset = Convert.ToInt32(1.5 * Game.FontSizeMultiplier);
        _positionOffset = (offset, offset);
    }

    public Point Start { get; set; } = Point.Zero;

    protected double Speed { get; set; } = 1d;

    protected int AnimationRow
    {
        set
        {
            _firstFrameIndex = value * 4;
        }
    }

    // direction in which the sprite is currently going
    public Direction Direction
    {
        get => _direction;
        protected set
        {
            var prevDirection = _direction;
            _direction = value;
            OnDirectionChanged(prevDirection, _direction);
        }
    }

    public Direction NextDirection
    {
        get => _nextDirection;
        set
        {
            var prevNextDirection = _nextDirection;
            _nextDirection = value;
            OnNextDirectionChanged(prevNextDirection, _nextDirection);
        }
    }

    // surface cell pixel position from where the sprite is going
    public Point FromPosition
    {
        get => _fromPosition;
        protected set
        {
            _fromPosition = value;
        }
    }

    // surface cell pixel position to where the sprite is heading
    public Point ToPosition
    {
        get => _toPosition;
        protected set
        {
            if (FromPosition == Point.None)
                throw new InvalidOperationException("FromPosition is not set.");
            else if (Direction == Direction.None)
                throw new InvalidOperationException("Direction is not set.");
            else
            {
                if (Direction == Direction.Up || Direction == Direction.Down)
                {
                    if (FromPosition.X != value.X)
                        throw new ArgumentException("Vertical movement can only happen in a straight line.");
                }
                else if (Direction == Direction.Left || Direction == Direction.Right)
                {
                    if (FromPosition.Y != value.Y)
                        throw new ArgumentException("Horizontal movement can only happen in a straight line.");
                }
                else
                    throw new InvalidOperationException("Only cardinal directions allowed.");
            }
            _toPosition = value;
        }
    }

    // current pixel position interpolating between FromPos and ToPos
    public Point CurrentPosition
    {
        get => _currentPosition;
        private set
        {
            var prevCurPos = _currentPosition;
            _currentPosition = value;
            OnCurrentPositionChanged(prevCurPos, _currentPosition);
        }
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board board)
        {
            FontSize = DefaultFontSize;
            FromPosition = Start;
            CurrentPosition = FromPosition;
        }
        base.OnParentChanged(oldParent, newParent);
    }

    // used mainly for movement
    public void UpdatePosition()
    {
        if (FromPosition != ToPosition && Direction != Direction.None)
        {
            _distanceTravelled += Speed;
            if (_distanceTravelled > 1)
            {
                double reminder = _distanceTravelled % 1;
                int distance = Convert.ToInt32(_distanceTravelled - reminder);
                _distanceTravelled = reminder;

                int x = Direction.DeltaX * distance;
                int y = Direction.DeltaY * distance;
                CurrentPosition += (x, y);
            }
        }
    }

    // used mainly to display sprite animation
    public override void Update(TimeSpan delta)
    {
        _timeElapsed += delta;
        if (_timeElapsed >= _animationFreq)
        {
            _timeElapsed = TimeSpan.Zero;
            Surface[0].Glyph = _firstFrameIndex + _animationColumn + _currentAnimFrame * 4;
            _currentAnimFrame = _currentAnimFrame == 0 ? 1 : 0;
            Surface.IsDirty = true;
        }
        base.Update(delta);
    }

    virtual protected void OnToPositionReached()
    {
        ToPositionReached?.Invoke(this, EventArgs.Empty);
    }

    virtual protected void OnCurrentPositionChanged(Point prevCurPos, Point newCurPos)
    {
        // move the sprite to a new position
        Position = CurrentPosition - _positionOffset;

        // calculate distance to the destination
        int distance = Direction.Type switch
        {
            Direction.Types.Up => CurrentPosition.Y - ToPosition.Y,
            Direction.Types.Down => ToPosition.Y - CurrentPosition.Y,
            Direction.Types.Left => CurrentPosition.X - ToPosition.X,
            Direction.Types.Right => ToPosition.X - CurrentPosition.X,
            _ => int.MaxValue
        };

        // check if the destination is reached
        if (distance <= 0)
        {
            FromPosition = ToPosition;
            OnToPositionReached();
        }

        // invoke event
        CurrentPositionChanged?.Invoke(this, EventArgs.Empty);
    }

    virtual protected void OnDirectionChanged(Direction prevDirection, Direction newDirection)
    {
        if (newDirection != Direction.None)
        {
            _animationColumn = newDirection.Type switch
            {
                Direction.Types.Up => 2,
                Direction.Types.Down => 3,
                Direction.Types.Left => 1,
                _ => 0
            };
        }
        DirectionChanged?.Invoke(this, EventArgs.Empty);
    }

    virtual protected void OnNextDirectionChanged(Direction prevNextDirection, Direction newNextDirection)
    {
        if (newNextDirection != Direction.None)
        {
            if (Direction != Direction.None)
            {
                // check for reversing current direction
                if (newNextDirection == Direction.Inverse())
                {
                    Direction = newNextDirection;
                    NextDirection = Direction.None;
                    var temp = FromPosition;
                    FromPosition = ToPosition;
                    ToPosition = temp;
                }
            }
            // check for stopped situations
            else if (FromPosition == ToPosition && Parent is Board board)
            {
                var position = board.GetNextPosition(FromPosition, newNextDirection);
                if (position != FromPosition)
                {
                    Direction = newNextDirection;
                    NextDirection = Direction.None;
                    ToPosition = position;
                }
            }
        }
    }

    public event EventHandler? ToPositionReached;
    public event EventHandler? CurrentPositionChanged;
    public event EventHandler? DirectionChanged;
}