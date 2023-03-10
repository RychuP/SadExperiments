using SadExperiments.Games.PacMan.Ghosts;

namespace SadExperiments.Games.PacMan;

// occupies the center of the board (size 7 x 4)
class GhostHouse
{
    readonly Point _centerPosition;

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
    }

    public Blinky Blinky { get; init; }
    public Inky Inky { get; init; }
    public Pinky Pinky { get; init; }
    public Clyde Clyde { get; init; }
    public Point EntrancePosition { get; init; }
    public Ghost[] Ghosts { get; init; }
}