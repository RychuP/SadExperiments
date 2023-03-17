using SadConsole.Components;
using SadExperiments.Games.PacMan.Ghosts;

namespace SadExperiments.Games.PacMan;

// occupies the center of the board (size 7 x 4)
class GhostHouse : ScreenObject
{
    #region Fields
    const int Width = 7;
    const int Height = 4;
    readonly Board _board;

    // power dot timer
    readonly Timer _powerDotTimer = new(TimeSpan.FromSeconds(5d));
    const double FrightenedTime = 6d;       // default time how long the ghosts will remain freightened
    const double TransitionTime = 3d;       // how long the ghosts will be flashing white
    int _ghostsEaten = 0;

    // ghost mode change change timer
    readonly Timer _modeTimer = new(TimeSpan.FromSeconds(5d));
    readonly List<Timer> _pausedTimers = new(3);
    const int ModeChangeMax = 7;
    int _currentModeTimeSlot = 0;
    GhostMode _currentMode = GhostMode.Scatter;
    readonly double[,] _modeTimeSlots = new double[,]
    {
        { 7, 20, 7, 20, 5, 20, 5 },         // level 1
        { 7, 20, 7, 20, 5, 1033, 1/60d },   // levels 2 - 4
        { 5, 20, 5, 20, 5, 1037, 1/60d },   // levels 5 - 21
    };

    // ghost release timer
    readonly Timer _releaseTimer = new(TimeSpan.FromSeconds(5d));
    readonly double[] _releaseTimes = { 2, 8, 8 };
    int _releaseCounter = 0;
    #endregion Fields

    #region Constructors
    public GhostHouse(Board board, Point spawnerPosition)
    {
        // calculate positions
        var fontSize = Game.DefaultFontSize;
        Point pixelPos = spawnerPosition.SurfaceLocationToPixel(fontSize);
        CenterSpot = pixelPos + (3, 1) * fontSize + (0, fontSize.Y / 2);
        LeftSpot = CenterSpot - (2, 0) * fontSize + (Convert.ToInt32(fontSize.X * 0.4d), 0);
        RightSpot = CenterSpot + (2, 0) * fontSize - (Convert.ToInt32(fontSize.X * 0.4d), 0);
        EntrancePosition = spawnerPosition + (3, -1);

        // calculate area
        PixelArea = new(pixelPos.X, pixelPos.Y, Width * fontSize.X, Height * fontSize.Y);
        TileArea = new(spawnerPosition.X, spawnerPosition.Y, Width, Height);

        // create ghosts
        Blinky = new(board, EntrancePosition);
        Pinky = new(board, CenterSpot);
        Inky = new(board, LeftSpot);
        Clyde = new(board, RightSpot);
        Ghosts = new Ghost[] { Blinky, Pinky, Inky, Clyde };
        foreach (var ghost in Ghosts)
            ghost.ModeChanged += Ghost_OnModeChanged;

        // add timers
        AddTimerToSadComponents(_powerDotTimer, PowerDotTimer_OnTimerElapsed);
        AddTimerToSadComponents(_modeTimer, ModeTimer_OnTimerElapsed);
        AddTimerToSadComponents(_releaseTimer, ReleaseTimer_OnTimerElapsed);

        // board setup
        _board = board;
        board.DotEaten += Board_OnDotEaten;
        board.GhostEaten += Board_OnGhostEaten;
        board.LiveLost += Board_OnLiveLost;
        board.Paused += Board_OnPaused;
        board.Resumed += Board_OnResumed;
    }
    #endregion Constructors

    #region Properties
    public Point EntrancePosition { get; init; }    // in cells
    public Point CenterSpot { get; init; }          // in pixels
    public Point LeftSpot { get; init; }            // in pixels
    public Point RightSpot { get; init; }           // in pixels
    public Blinky Blinky { get; init; }
    public Inky Inky { get; init; }
    public Pinky Pinky { get; init; }
    public Clyde Clyde { get; init; }
    public Ghost[] Ghosts { get; init; }
    public bool PowerDotTransition { get; private set; } = false;
    public int ModeChangesCount =>
        _currentModeTimeSlot;

    public GhostMode CurrentMode
    {
        get => _currentMode;
        set
        {
            if (_currentMode == value) return;
            var prevMode = _currentMode;
            _currentMode = value;
            OnCurrentModeChanged(prevMode, _currentMode);
        }
    }

    // off limits for scatter behaviour
    public Rectangle TileArea { get; init; }
    public Rectangle PixelArea { get; init; }

    // value of the last ghost eaten
    public int Value
    {
        get
        {
            int value = (int)Math.Pow(2, _ghostsEaten + 1) * 100;
            return value;
        }
    }
    #endregion Properties

    #region Methods
    void AddTimerToSadComponents(Timer timer, EventHandler handler)
    {
        timer.Repeat = false;
        timer.IsPaused = true;
        timer.TimerElapsed += handler;
        SadComponents.Add(timer);
    }

    double GetPowerDotTime() =>
        ((double)(Game.MaxDifficultyLevel - _board.Game.Level + 1) / Game.MaxDifficultyLevel) * FrightenedTime;

    // initial timers to start when a level loads or restarts
    public void StartTimers()
    {
        // mode timer
        _currentModeTimeSlot = 0;
        SetModeTimer(_board.Game.Level);
        _modeTimer.Restart();

        // release timer
        _releaseCounter = 0;
        _releaseTimer.TimerAmount = TimeSpan.FromSeconds(_releaseTimes[0]);
        _releaseTimer.Restart();
    }

    void StopTimers()
    {
        foreach (var component in SadComponents)
            if (component is Timer timer)
                timer.IsPaused = true;
    }

    public void PauseRunningTimers()
    {
        _pausedTimers.Clear();

        foreach (var component in SadComponents)
        {
            if (component is Timer timer)
            {
                if (!timer.IsPaused)
                {
                    timer.IsPaused = true;
                    _pausedTimers.Add(timer);
                }
            }
        }
    }

    public void ResumePausedTimers()
    {
        foreach (var timer in _pausedTimers)
            timer.IsPaused = false;
        _pausedTimers.Clear();
    }

    void SetModeTimer(int gameLevel)
    {
        if (_currentModeTimeSlot < 0 || _currentModeTimeSlot >= _modeTimeSlots.Length)
            throw new IndexOutOfRangeException("Invalid index.");

        _modeTimer.TimerAmount = TimeSpan.FromSeconds(gameLevel switch
        {
            >= 5 => _modeTimeSlots[2, _currentModeTimeSlot],
            >= 2 => _modeTimeSlots[1, _currentModeTimeSlot],
            _ => _modeTimeSlots[0, _currentModeTimeSlot]
        });
    }

    void Board_OnGhostEaten(object? o, GhostEventArgs e)
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

    void Board_OnPaused(object? o, EventArgs e)
    {
        PauseRunningTimers();
    }

    void Board_OnResumed(object? o, EventArgs e)
    {
        ResumePausedTimers();
    }

    void PowerDotTimer_OnTimerElapsed(object? o, EventArgs e)
    {
        // check if blue time was on and if so start the transition period
        if (!PowerDotTransition)
        {
            PowerDotTransition = true;
            _powerDotTimer.TimerAmount = TimeSpan.FromSeconds(TransitionTime);
            _powerDotTimer.Restart();
        }

        // transition finished -> put all ghosts into the current mode
        else
        {
            foreach (var ghost in Ghosts)
                if (ghost.Mode != GhostMode.Eaten && ghost.Mode != GhostMode.Idle)
                    ghost.Mode = CurrentMode;
            OnPowerDotDepleted();
        }
    }

    void ModeTimer_OnTimerElapsed(object? o, EventArgs e)
    {
        if (++_currentModeTimeSlot >= ModeChangeMax)
        {
            // indefinite chase mode
            CurrentMode = GhostMode.Chase;
            _modeTimer.IsPaused = true;
            return;
        }

        // set mode
        CurrentMode = CurrentMode == GhostMode.Scatter ? GhostMode.Chase : GhostMode.Scatter;

        // set time
         SetModeTimer(_board.Game.Level);

        _modeTimer.Restart();
    }

    void ReleaseTimer_OnTimerElapsed(object? o, EventArgs e)
    {
        // pick the ghost
        Ghost ghost = _releaseCounter switch
        {
            0 => Pinky,
            1 => Inky,
            _ => Clyde
        };

        // set the mode
        ghost.Mode = GhostMode.Awake;

        // when all ghosts are out stop the timer
        if (++_releaseCounter >= _releaseTimes.Length)
            _releaseTimer.IsPaused = true;
        else
        {
            // set the new release time and restart timer
            _releaseTimer.TimerAmount = TimeSpan.FromSeconds(_releaseTimes[_releaseCounter]);
            _releaseTimer.Restart();
        }
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
        if (newParent is Board)
        {
            CurrentMode = GhostMode.Scatter;
            _board.Children.Add(Ghosts);
            StartTimers();
        }

        else
        {
            foreach (var ghost in Ghosts)
                _board.Children.Remove(ghost);
            StopTimers();
        }

        base.OnParentChanged(oldParent, newParent);
    }

    void OnCurrentModeChanged(GhostMode prevMode, GhostMode newMode)
    {
        foreach (var ghost in Ghosts)
        {
            if (ghost.Mode == GhostMode.Scatter || ghost.Mode == GhostMode.Chase)
                ghost.Mode = newMode;
        }

        // debug
        if (_board.IsDebugging && prevMode == GhostMode.Chase)
            _board.TurnOffHighlight();

        var args = new GhostModeEventArgs(prevMode, newMode);
        ModeChanged?.Invoke(this, args);
    }

    void OnPowerDotDepleted()
    {
        _powerDotTimer.IsPaused = true;
        PowerDotDepleted?.Invoke(this, EventArgs.Empty);
    }

    void OnPowerDotStarted()
    {
        if (_board.Game.Level < Game.MaxDifficultyLevel)
        {
            // calculate power dot time
            double amount = GetPowerDotTime();

            // set transition
            if (amount > TransitionTime)
            {
                PowerDotTransition = false;
                amount -= TransitionTime;
            }
            else
                PowerDotTransition = true;

            _powerDotTimer.TimerAmount = TimeSpan.FromSeconds(amount);
            _powerDotTimer.Restart();


            // set counter
            _ghostsEaten = 0;

            // place all ghosts outside the ghost house into a frightened mode
            foreach (var ghost in Ghosts)
                if (!ghost.IsInGhostHouse())
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
    public event EventHandler<GhostModeEventArgs>? ModeChanged;
    #endregion Events
}

class GhostModeEventArgs : EventArgs
{
    public GhostMode PrevMode { get; init; }
    public GhostMode NewMode { get; init; }
    public GhostModeEventArgs(GhostMode prevMode, GhostMode newMode) =>
        (PrevMode, NewMode) = (prevMode, newMode);
}