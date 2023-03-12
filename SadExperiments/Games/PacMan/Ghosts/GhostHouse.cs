using SadConsole.Components;
using SadExperiments.Games.PacMan.Ghosts;

namespace SadExperiments.Games.PacMan;

// occupies the center of the board (size 7 x 4)
class GhostHouse : ScreenObject
{
    #region Fields
    const int Width = 7;
    const int Height = 4;
    const double FrightenedTime = 3d;
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

        // event handlers
        _timer.Repeat = false;
        _timer.IsPaused = true;
        _timer.TimerElapsed += Timer_OnTimerElapsed;
        SadComponents.Add(_timer);
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

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board board)
        {
            board.DotEaten += Board_OnDotEaten;
            board.LiveLost += Board_OnLiveLost;
        }
        base.OnParentChanged(oldParent, newParent);
    }

    void Board_OnGhostEaten(object? o, EventArgs e)
    {
        if (++_ghostsEaten == 3)
            OnPowerDotDepleted();
    }

    void Board_OnLiveLost(object? o, EventArgs e)
    {
        _timer.IsPaused = true;
    }

    void Board_OnDotEaten(object? o, DotEventArgs e)
    {
        if (e.Dot is not PowerDot) return;

        _timer.Restart();
        _ghostsEaten = 0;
        Sounds.Siren.Stop();
        Sounds.PowerDot.Play();

        // place all ghosts outside the ghost house into a frightened mode
        foreach (var ghost in Ghosts)
            if (!IsInside(ghost))
                ghost.Mode = GhostMode.Frightened;

        // check if any ghost entered a frightened mode
        if (!Ghosts.Any(g => g.Mode == GhostMode.Frightened))
            OnPowerDotDepleted();
    }

    void Timer_OnTimerElapsed(object? o, EventArgs e)
    {
        foreach (var ghost in Ghosts)
            if (ghost.Mode != GhostMode.Eaten && ghost.Mode != GhostMode.Idle)
                ghost.Mode = GhostMode.Chase;
        OnPowerDotDepleted();
    }

    void OnPowerDotDepleted()
    {
        _timer.IsPaused = true;
        Sounds.PowerDot.Stop();
        Sounds.Siren.Play();
        PowerDotDepleted?.Invoke(this, EventArgs.Empty);
    }
    #endregion Methods

    #region Events
    public event EventHandler? PowerDotDepleted;
    #endregion Events
}