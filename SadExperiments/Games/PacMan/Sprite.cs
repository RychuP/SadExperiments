using SadExperiments.Games.PacMan.Ghosts;

namespace SadExperiments.Games.PacMan;

abstract class Sprite : ScreenSurface
{
    #region Fields
    static readonly Point DefaultFontSize = Game.DefaultFontSize + new Point(3, 3) * Game.FontSizeMultiplier;

    // movement data
    Departure _departure;
    Destination _destination = Destination.None;
    Point _currentPosition = Point.None;
    double _distanceTravelled = 0;

    // sprites occupy more space than a tile (their actual position is offsetted a few pixels)
    readonly Point _positionOffset;

    // two frame animation data
    TimeSpan _animationSpeed = TimeSpan.FromSeconds(0.15d);
    TimeSpan _timeElapsed = TimeSpan.Zero;
    int _currentAnimFrame = 0;                                  // either 0 or 1
    int _animationColumn = 0;                                   // between 0 - 3 (represents cardinal directions)
    int _firstFrameIndex = 0;                                   // first frame on the left of the animation row
    #endregion Fields

    #region Constructors
    public Sprite(Board board, Point start) : base(1, 1)
    {
        Font = Fonts.Sprites;
        FontSize = DefaultFontSize;
        UsePixelPositioning = true;
        Start = start;
        Board = board;

        // calculate sprite position offset
        int offset = Convert.ToInt32(1.5 * Game.FontSizeMultiplier);
        _positionOffset = (offset, offset);

        // collision detector
        HitBox = new Rectangle(0, 0, Game.DefaultFontSize.X, Game.DefaultFontSize.Y);
    }
    #endregion Constructors

    #region Properties
    protected Board Board { get; init; }
    public Point Start { get; init; } 
    public double Speed { get; protected set; } = Game.SpriteSpeed;
    public Rectangle HitBox { get; private set; }

    protected int AnimationRow
        { set => _firstFrameIndex = value * 4; }

    protected double AnimationSpeed
        { set => _animationSpeed = TimeSpan.FromSeconds(value); }

    protected int AnimationColumn
        {  set => _animationColumn = value; }

    public Departure Departure
    {
        get => _departure;
        protected set
        {
            if (_departure == value) return;

            var prevDeparture = _departure;
            _departure = value;

            OnDepartureChanged(prevDeparture, _departure);
        }
    }

    public Destination Destination
    {
        get => _destination;
        protected set
        {
            if (_destination == value) 
                return;

            else if (value != Destination.None &&
                value.Position != Board.GhostHouse.PinkyPosition &&       // ignore ghosts position
                value.Position != Board.GhostHouse.InkyPosition &&        // which are pixel
                value.Position != Board.GhostHouse.ClydePosition)
            {
                if (Departure == Departure.None)
                    throw new InvalidOperationException("Departure is not set.");

                else if (Departure.Position != Board.GhostHouse.PinkyPosition)
                {
                    if (value.Direction == Direction.Up || value.Direction == Direction.Down)
                    {
                        if (Departure.Position.X != value.Position.X)
                            throw new ArgumentException("Vertical movement can only happen in a straight line.");
                    }
                    else if (value.Direction == Direction.Left || value.Direction == Direction.Right)
                    {
                        if (Departure.Position.Y != value.Position.Y)
                            throw new ArgumentException("Horizontal movement can only happen in a straight line.");
                    }
                }
            }

            var prevDestination = _destination;
            _destination = value;

            OnDestinationChanged(prevDestination, _destination);
        }
    }

    // current pixel position interpolating between departure and destination
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
    #endregion Properties

    #region Methods
    public void UpdatePosition()
    {
        if (Departure != Departure.None && Destination != Destination.None && Departure != Destination)
        {
            _distanceTravelled += Speed;

            if (_distanceTravelled > 1)
            {
                double reminder = _distanceTravelled % 1;
                int distance = Convert.ToInt32(_distanceTravelled - reminder);
                _distanceTravelled = reminder;

                int x = Destination.Direction.DeltaX * distance;
                int y = Destination.Direction.DeltaY * distance;
                CurrentPosition += (x, y);
            }
        }
    }

    public virtual void UpdateAnimation(TimeSpan delta)
    {
        _timeElapsed += delta;
        if (_timeElapsed >= _animationSpeed)
        {
            _timeElapsed = TimeSpan.Zero;
            Surface[0].Glyph = GetAnimationGlyph(_animationColumn, _currentAnimFrame);
            _currentAnimFrame = _currentAnimFrame == 0 ? 1 : 0;
            Surface.IsDirty = true;
        }
    }

    protected void SetCurrentAnimationGlyph() =>
        Surface[0].Glyph = GetAnimationGlyph(_animationColumn, _currentAnimFrame);

    // extracted to a method to allow for a custom freightened ghost animation
    protected virtual int GetAnimationGlyph(int animationColumn, int animationFrame) =>
        _firstFrameIndex + animationColumn + animationFrame * 4;

    protected bool TrySetDestination(Direction direction)
    {
        if (direction == Direction.None)
            throw new ArgumentException("Direction is required.");

        if (Departure == Departure.None)
            throw new InvalidOperationException("Departure is not set.");

        var position = Board.GetNextPosition(Departure.Position, direction);
        if (position != Departure.Position)
        {
            Destination = new Destination(position, direction);
            return true;
        }
        else
            return false;
    }

    virtual protected void OnDepartureChanged(Departure prevDeparture, Departure newDeparture)
    {
        if (newDeparture.Position != Destination.Position)
        {
            if (Departure.Position == Board.GhostHouse.PinkyPosition ||
                Departure.Position == Board.GhostHouse.InkyPosition ||
                Departure.Position == Board.GhostHouse.ClydePosition)
                    CurrentPosition = newDeparture.Position;
             else
                CurrentPosition = newDeparture.PixelPosition;
        }
    }

    virtual protected void OnCurrentPositionChanged(Point prevPosition, Point newPosition)
    {
        if (newPosition == Point.None || prevPosition == newPosition)
            return;

        // move the sprite to a new position
        Position = newPosition - _positionOffset;

        // calculate distance to the destination
        if (Destination != Destination.None && Departure != Destination)
        {
            var destination = (Destination.Position == Board.GhostHouse.PinkyPosition ||
                Destination.Position == Board.GhostHouse.InkyPosition ||
                Destination.Position == Board.GhostHouse.ClydePosition) ?
                    Destination.Position : Destination.PixelPosition;

            int distance = Destination.Direction.Type switch
            {
                Direction.Types.Up => newPosition.Y - destination.Y,
                Direction.Types.Down => destination.Y - newPosition.Y,
                Direction.Types.Left => newPosition.X - destination.X,
                Direction.Types.Right => destination.X - newPosition.X,
                _ => int.MaxValue
            };

            // check if the destination is reached
            if (distance <= 0)
            {
                Departure = new(Destination.Position);
                OnDestinationReached(Destination);
            }
        }

        // move the hit box
        HitBox = HitBox.WithPosition(newPosition);

        // invoke event
        CurrentPositionChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnDestinationChanged(Destination prevDestination, Destination newDestination)
    {
        if (newDestination.Direction != Direction.None)
        {
            _animationColumn = newDestination.Direction.Type switch
            {
                Direction.Types.Up => 2,
                Direction.Types.Down => 3,
                Direction.Types.Left => 1,
                _ => 0
            };
        }
    }

    virtual protected void OnDestinationReached(Destination destination)
    {
        DestinationReached?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board)
        {
            Departure = new(Start);
            Surface[0].Glyph = GetAnimationGlyph(_animationColumn, _currentAnimFrame);
            Surface.IsDirty = true;
        }
        else
        {
            Departure = Departure.None;
            Destination = Destination.None;
        }

        base.OnParentChanged(oldParent, newParent);
    }
    #endregion Methods

    #region Events
    public event EventHandler? DestinationReached;
    public event EventHandler? CurrentPositionChanged;
    #endregion Events
}