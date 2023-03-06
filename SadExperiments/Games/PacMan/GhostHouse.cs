namespace SadExperiments.Games.PacMan;

// occupies the center of the board (size 7 x 4)
class GhostHouse
{
    readonly Point _centerPosition;
    readonly Point _entrancePosition;

    public GhostHouse(Point spawnerPosition)
    {
        Point position = spawnerPosition.SurfaceLocationToPixel(Game.DefaultFontSize);
        _entrancePosition = position + (3, -1) * Game.DefaultFontSize;
        _centerPosition = position + (3, 1) * Game.DefaultFontSize + (0, Game.DefaultFontSize.Y / 2);
    }

    public Point Entrance => _entrancePosition;
    public Point BlinkyPosition => _entrancePosition;
    public Point PinkyPosition => _centerPosition;
    public Point InkyPosition => _centerPosition - (2, 0) * Game.DefaultFontSize + 
        (Convert.ToInt32((double)Game.DefaultFontSize.X * 0.4d), 0);
    public Point ClydePosition => _centerPosition + (2, 0) * Game.DefaultFontSize -
        (Convert.ToInt32((double)Game.DefaultFontSize.X * 0.4d), 0);
}