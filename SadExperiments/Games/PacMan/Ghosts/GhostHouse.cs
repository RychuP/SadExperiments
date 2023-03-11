using SadConsole.Components;
using SadExperiments.Games.PacMan.Ghosts;

namespace SadExperiments.Games.PacMan;

// occupies the center of the board (size 7 x 4)
class GhostHouse : ScreenObject
{
    #region Fields
    const double FrightenedTime = 8d;
    readonly Point _centerPosition;
    readonly Timer _timer = new(TimeSpan.FromSeconds(FrightenedTime));
    #endregion Fields

    #region Constructors
    public GhostHouse(Point spawnerPosition)
    {
        // calculate positions
        var fontSize = Game.DefaultFontSize;
        Point position = spawnerPosition.SurfaceLocationToPixel(fontSize);
        EntrancePosition = position + (3, -1) * fontSize;
        _centerPosition = position + (3, 1) * fontSize + (0, fontSize.Y / 2);

        // create ghosts
        Blinky = new(EntrancePosition);
        Pinky = new(_centerPosition);
        Inky = new(_centerPosition - (2, 0) * fontSize + (Convert.ToInt32(fontSize.X * 0.4d), 0));
        Clyde = new(_centerPosition + (2, 0) * fontSize - (Convert.ToInt32(fontSize.X * 0.4d), 0));
        Ghosts = new Ghost[] { Blinky, Pinky, Inky, Clyde };

        // event handlers
        _timer.TimerElapsed += Timer_OnTimerElapsed;
    }
    #endregion Constructors

    #region Properties
    public Blinky Blinky { get; init; }
    public Inky Inky { get; init; }
    public Pinky Pinky { get; init; }
    public Clyde Clyde { get; init; }
    public Point EntrancePosition { get; init; }
    public Ghost[] Ghosts { get; init; }
    #endregion Properties

    #region Methods
    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Board board)
        {
            board.DotEaten += Board_OnDotEaten;
        }
        base.OnParentChanged(oldParent, newParent);
    }

    void Board_OnDotEaten(object? o, DotEventArgs e)
    {
        if (e.Dot is not PowerDot && Parent is not Board && Parent.Parent is not Game game) return;


    }

    void Timer_OnTimerElapsed(object? o, EventArgs e)
    {
        int i = 0;
    }
    #endregion Methods
}