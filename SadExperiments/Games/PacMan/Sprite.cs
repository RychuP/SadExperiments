using Newtonsoft.Json.Linq;
using SadExperiments.Games.PacMan.Ghosts;
using SadExperiments.Games.Tetris;

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
    bool _isTeleporting = false;

    // sprites occupy more space than a board tile (their position is offsetted a few pixels)
    readonly Point _positionOffset;

    // two frame animation data
    TimeSpan _animationSpeed = TimeSpan.FromSeconds((double)8 / 60);
    TimeSpan _timeElapsed = TimeSpan.Zero;
    int _currentAnimFrame = 0;
    int _animationColumn = 0;
    // first frame on the left of the animation row
    int _firstFrameIndex = 0;

    public Sprite(Point start) : base(1, 1)
    {
        Font = Fonts.Sprites;
        FontSize = DefaultFontSize;
        UsePixelPositioning = true;
        Start = start;

        // calculate sprite position offset
        int offset = Convert.ToInt32(1.5 * Game.FontSizeMultiplier);
        _positionOffset = (offset, offset);

        HitBox = new Rectangle(0, 0, Game.DefaultFontSize.X, Game.DefaultFontSize.Y);
    }

    public Point Start { get; set; } = Point.Zero;

    protected double Speed { get; set; } = 1d;

    public Rectangle HitBox { get; private set; }

    public bool AnimationIsOn { get; set; } = true;

    protected int AnimationRow
    {
        set => _firstFrameIndex = value * 4;
    }

    protected double AnimationSpeed
    {
        set => _animationSpeed = TimeSpan.FromSeconds(value);
    }

    // direction in which the sprite is currently going
    public Direction Direction
    {
        get => _direction;
        protected set
        {
            if (_direction == value) return;
            if (_isTeleporting) return;

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
            if (_nextDirection == value) return;
            if (_isTeleporting) return;

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
            if (_fromPosition == value) return;
            
            _fromPosition = value;
        }
    }

    // surface cell pixel position to where the sprite is heading
    public Point ToPosition
    {
        get => _toPosition;
        protected set
        {
            if (_toPosition == value) return;
            else if (value != Point.None)
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
            if (_currentPosition == value) return;

            var prevCurPos = _currentPosition;
            _currentPosition = value;
            OnCurrentPositionChanged(prevCurPos, _currentPosition);
        }
    }

    // used for movement
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

    // used to display sprite animation
    public override void Update(TimeSpan delta)
    {
        if (!AnimationIsOn) return;

        _timeElapsed += delta;
        if (_timeElapsed >= _animationSpeed)
        {
            _timeElapsed = TimeSpan.Zero;
            Surface[0].Glyph = GetAnimationGlyph(_animationColumn, _currentAnimFrame);
            _currentAnimFrame = _currentAnimFrame == 0 ? 1 : 0;
            Surface.IsDirty = true;
        }

        base.Update(delta);
    }

    // extracted to a method to allow for a custom freightened ghost animation
    protected virtual int GetAnimationGlyph(int animationColumn, int animationFrame) =>
        _firstFrameIndex + animationColumn + animationFrame * 4;

    protected bool TrySetDestination(Direction direction)
    {
        if (Parent is not Board board)
            throw new InvalidOperationException("Trying to find a destination when not assigned to a board.");

        var position = board.GetNextPosition(FromPosition, direction);
        if (position != FromPosition)
        {
            Direction = direction;
            ToPosition = position;
            return true;
        }
        else
            return false;
    }

    virtual protected void OnToPositionReached()
    {
        if (Parent is not Board board) return;

        if (board.IsPortal(FromPosition, out Portal? destination))
        {
            if (destination == null) return;
            _isTeleporting = true;
            ToPosition = Point.None;
            FromPosition = CurrentPosition = destination.Position.SurfaceLocationToPixel(board.FontSize);
        }
        else if (_isTeleporting)
            _isTeleporting = false;

        ToPositionReached?.Invoke(this, EventArgs.Empty);
    }

    virtual protected void OnCurrentPositionChanged(Point prevCurPos, Point newCurPos)
    {
        // move the sprite to a new position
        Position = newCurPos - _positionOffset;

        // calculate distance to the destination
        if (ToPosition != Point.None)
        {
            int distance = Direction.Type switch
            {
                Direction.Types.Up => newCurPos.Y - ToPosition.Y,
                Direction.Types.Down => ToPosition.Y - newCurPos.Y,
                Direction.Types.Left => newCurPos.X - ToPosition.X,
                Direction.Types.Right => ToPosition.X - newCurPos.X,
                _ => int.MaxValue
            };

            // check if the destination is reached
            if (distance <= 0)
            {
                FromPosition = ToPosition;
                OnToPositionReached();
            }
        }

        // move the hit box
        HitBox = HitBox.WithPosition(newCurPos);

        // invoke event
        CurrentPositionChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board)
        {
            AnimationIsOn = true;
            FromPosition = CurrentPosition = Start;
            if (Direction != Direction.None && !TrySetDestination(Direction))
                throw new InvalidOperationException("Invalid start. Sprite unable to go in the given direction.");
        }
        else if (oldParent is Board)
        {
            _fromPosition = Point.None;
            _toPosition = Point.None;
            _currentPosition = Point.None;
        }
        base.OnParentChanged(oldParent, newParent);
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

    // used only with the player sprite
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