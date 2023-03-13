using SadConsole.Components;
using SadConsole.Effects;
using SadExperiments.Games.PacMan.Ghosts;

namespace SadExperiments.Games.PacMan;

// occupies the center of the board (size 7 x 4)
class GhostHouse : ScreenObject
{
    #region Fields
    const int Width = 7;
    const int Height = 4;
    const double FrightenedTime = 6d;
    readonly Timer _timer = new(TimeSpan.FromSeconds(FrightenedTime));
    readonly Rectangle _area;
    int _ghostsEaten = 0;
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

        // setup timer 
        _timer.Repeat = false;
        _timer.IsPaused = true;
        _timer.TimerElapsed += Timer_OnTimerElapsed;
        SadComponents.Add(_timer);

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

    // value of the last ghost eaten
    public int Value =>
        (int)Math.Pow(2, _ghostsEaten + 1) * 100;
    #endregion Properties

    #region Methods
    // checks if the ghost is inside the ghost house
    bool IsInside(Ghost ghost) =>
        _area.Intersects(ghost.HitBox);

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
        if (!_timer.IsPaused)
            OnPowerDotDepleted();
    }

    void Board_OnDotEaten(object? o, DotEventArgs e)
    {
        if (e.Dot is PowerDot)
            OnPowerDotStarted();
    }

    void Timer_OnTimerElapsed(object? o, EventArgs e)
    {
        foreach (var ghost in Ghosts)
            if (ghost.Mode != GhostMode.Eaten && ghost.Mode != GhostMode.Idle)
                ghost.Mode = GhostMode.Chase;
        OnPowerDotDepleted();
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
        if (newParent is Board board)
        {
            board.DotEaten += Board_OnDotEaten;
            board.GhostEaten += Board_OnGhostEaten;
            board.LiveLost += Board_OnLiveLost;
        }
        base.OnParentChanged(oldParent, newParent);
    }

    void OnPowerDotDepleted()
    {
        _timer.IsPaused = true;
        PowerDotDepleted?.Invoke(this, EventArgs.Empty);
    }

    void OnPowerDotStarted()
    {
        if (Parent is not Board board || board.Parent is not Game game) return;

        if (game.Level < Game.MaxDifficultyLevel)
        {
            // start power dot timer
            double amount = (double)(Game.MaxDifficultyLevel - game.Level + 1) / Game.MaxDifficultyLevel;
            _timer.TimerAmount = TimeSpan.FromSeconds(FrightenedTime * amount);
            _timer.Restart();

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