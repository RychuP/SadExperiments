using SadConsole.Components;
using SadExperiments.Games.PacMan.Ghosts;

namespace SadExperiments.Games.PacMan;

// occupies the center of the board (size 7 x 4)
class GhostHouse : ScreenObject
{
    #region Fields
    const int Width = 7;
    const int Height = 4;
    readonly Rectangle _area;

    // power dot timer
    readonly Timer _powerDotTimer = new(TimeSpan.FromSeconds(5d));
    const double FrightenedTime = 6d;
    int _ghostsEaten = 0;

    // ghost mode change change timer
    readonly Timer _modeTimer = new(TimeSpan.FromSeconds(5d));
    const int ModeChangeMax = 7;
    int _currentModeTimeSlot = 0;
    GhostMode _currentMode = GhostMode.Scatter;
    readonly double[,] _modeTimeSlots = new double[,]
    {
        { 7, 20, 7, 20, 5, 20, 5 },
        { 7, 20, 7, 20, 5, 1033, 1/60d },
        { 5, 20, 5, 20, 5, 1037, 1/60d },
    };

    // ghost release timer
    readonly Timer _releaseTimer = new(TimeSpan.FromSeconds(5d));
    #endregion Fields

    #region Constructors
    public GhostHouse(Point spawnerPosition)
    {
        // calculate positions
        var fontSize = Game.DefaultFontSize;
        Point position = spawnerPosition.SurfaceLocationToPixel(fontSize);
        EntrancePosition = position + (3, -1) * fontSize;
        CenterPosition = position + (3, 1) * fontSize + (0, fontSize.Y / 2);

        // calculate area
        _area = new(position.X, position.Y, Width * fontSize.X, Height * fontSize.Y);

        // create ghosts
        Blinky = new(EntrancePosition);
        Pinky = new(CenterPosition);
        Inky = new(CenterPosition - (2, 0) * fontSize + (Convert.ToInt32(fontSize.X * 0.4d), 0));
        Clyde = new(CenterPosition + (2, 0) * fontSize - (Convert.ToInt32(fontSize.X * 0.4d), 0));
        Ghosts = new Ghost[] { Blinky, Pinky, Inky, Clyde };

        // setup power dot timer 
        _powerDotTimer.Repeat = false;
        _powerDotTimer.IsPaused = true;
        _powerDotTimer.TimerElapsed += PowerDotTimer_OnTimerElapsed;
        SadComponents.Add(_powerDotTimer);

        // setup ghost mode change timer
        _modeTimer.Repeat = false;
        _modeTimer.IsPaused = true;
        _modeTimer.TimerElapsed += ModeTimer_OnTimerElapsed;
        SadComponents.Add(_modeTimer);

        // event handlers
        foreach (var ghost in Ghosts)
            ghost.ModeChanged += Ghost_OnModeChanged;
    }
    #endregion Constructors

    #region Properties
    public Blinky Blinky { get; init; }
    public Inky Inky { get; init; }
    public Pinky Pinky { get; init; }
    public Clyde Clyde { get; init; }
    public Point EntrancePosition { get; init; }
    public Point CenterPosition { get; init; }
    public Ghost[] Ghosts { get; init; }
    public GhostMode CurrentMode
    {
        get => _currentMode;
        set
        {
            _currentMode = value;
            OnCurrentModeChanged(_currentMode);
        }
    }

    // value of the last ghost eaten
    public int Value =>
        (int)Math.Pow(2, _ghostsEaten + 1) * 100;
    #endregion Properties

    #region Methods
    // checks if the ghost is inside the ghost house
    bool IsInside(Ghost ghost) =>
        _area.Intersects(ghost.HitBox);

    void SetModeTimer(int gameLevel)
    {
        _modeTimer.TimerAmount = TimeSpan.FromSeconds(gameLevel switch
        {
            >= 5 => _modeTimeSlots[2, _currentModeTimeSlot],
            >= 2 => _modeTimeSlots[1, _currentModeTimeSlot],
            _ => _modeTimeSlots[0, _currentModeTimeSlot]
        });
    }

    public void Restart()
    {
        if (Parent is Board board && board.Parent is Game game)
        {
            _powerDotTimer.IsPaused = true;
            _modeTimer.Restart();
            _currentModeTimeSlot = 0;
            CurrentMode = GhostMode.Scatter;
            SetModeTimer(game.Level);
        }
    }

    void Board_OnGhostEaten(object? o, EventArgs e)
    {
        _ghostsEaten++;
        bool ghostsFrightenedRemain = Ghosts.Any(g => g.Mode == GhostMode.Frightened);

        // check if the ghosts eaten limit has been reached or if all ghosts respawned
        if (_ghostsEaten == 3 || !ghostsFrightenedRemain)
            OnPowerDotDepleted();
    }

    void Board_OnLiveLost(object? o, EventArgs e)
    {
        if (!_powerDotTimer.IsPaused)
            OnPowerDotDepleted();
        _modeTimer.IsPaused = true;
    }

    void Board_OnDotEaten(object? o, DotEventArgs e)
    {
        if (e.Dot is PowerDot)
            OnPowerDotStarted();
    }

    void Board_OnFirstStart(object? o, EventArgs e)
    {
        _modeTimer.Restart();
    }

    void PowerDotTimer_OnTimerElapsed(object? o, EventArgs e)
    {
        foreach (var ghost in Ghosts)
            if (ghost.Mode != GhostMode.Eaten && ghost.Mode != GhostMode.Idle)
                ghost.Mode = CurrentMode;
        OnPowerDotDepleted();
    }

    void ModeTimer_OnTimerElapsed(object? o, EventArgs e)
    {
        if (Parent is Board board && board.Parent is Game game)
        {
            if (++_currentModeTimeSlot >= ModeChangeMax)
            {
                // indefinite chase mode
                CurrentMode = GhostMode.Scatter;
                _modeTimer.IsPaused = true;
            }
            else
            {
                // set mode
                int reminder = _currentModeTimeSlot % 2;
                CurrentMode = reminder switch
                {
                    0 => GhostMode.Scatter,
                    _ => GhostMode.Chase
                };

                // set time
                SetModeTimer(game.Level);

                _modeTimer.Restart();
            }
        }
        else
            throw new InvalidOperationException("Timer is running but the board is not assigned to a Game.");
    }

    void Ghost_OnModeChanged(object? o, GhostModeEventArgs e)
    {
        // a ghost got eaten - play retreating sound
        if (e.NewMode == GhostMode.Eaten)
            Sounds.Retreating.Play();

        // a ghost respawned - check if there are any ghosts left in the eaten state
        else if (e.PrevMode == GhostMode.Eaten)
        {
            // no eaten ghosts left - stop the sound
            if (!Ghosts.Any(g => g.Mode == GhostMode.Eaten))
                Sounds.Retreating.Stop();
        }
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board board && board.Parent is not Game)
        {
            board.DotEaten += Board_OnDotEaten;
            board.GhostEaten += Board_OnGhostEaten;
            board.LiveLost += Board_OnLiveLost;
            board.FirstStart += Board_OnFirstStart;
        }
        base.OnParentChanged(oldParent, newParent);
    }

    void OnCurrentModeChanged(GhostMode newMode)
    {
        foreach (var ghost in Ghosts)
        {
            if (ghost.Mode == GhostMode.Scatter || ghost.Mode == GhostMode.Chase)
                ghost.Mode = newMode;
        }
    }

    void OnPowerDotDepleted()
    {
        _powerDotTimer.IsPaused = true;
        PowerDotDepleted?.Invoke(this, EventArgs.Empty);
    }

    void OnPowerDotStarted()
    {
        if (Parent is not Board board || board.Parent is not Game game) return;

        if (game.Level < Game.MaxDifficultyLevel)
        {
            // start power dot timer
            double amount = (double)(Game.MaxDifficultyLevel - game.Level + 1) / Game.MaxDifficultyLevel;
            _powerDotTimer.TimerAmount = TimeSpan.FromSeconds(FrightenedTime * amount);
            _powerDotTimer.Restart();

            // set counter
            _ghostsEaten = 0;

            // place all ghosts outside the ghost house into a frightened mode
            foreach (var ghost in Ghosts)
                if (!IsInside(ghost))
                    ghost.Mode = GhostMode.Frightened;

            // invoke event
            PowerDotStarted?.Invoke(this, EventArgs.Empty);

            // if no ghosts entered a frightened mode stop the event
            if (!Ghosts.Any(g => g.Mode == GhostMode.Frightened))
                OnPowerDotDepleted();
        }
    }
    #endregion Methods

    #region Events
    public event EventHandler? PowerDotDepleted;
    public event EventHandler? PowerDotStarted;
    #endregion Events
}