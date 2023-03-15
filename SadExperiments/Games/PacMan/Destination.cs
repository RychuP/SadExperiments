using System.Diagnostics.CodeAnalysis;

namespace SadExperiments.Games.PacMan;

// sprite destination position and direction from departure
readonly struct Destination : IEquatable<Destination>
{
    public static readonly Destination None = new();
    public Point Position { get; init; }
    public Point PixelPosition { get; init; } = Point.None;
    public Direction Direction { get; init; }

    public Destination()
    {
        Position = Point.None;
        Direction = Direction.None;
    }

    public Destination(Point position, Direction direction)
    {
        Position = position;

        if (position != Point.None && direction == Direction.None)
            throw new ArgumentException("Direction is required for the given position.");
        else if (!direction.IsCardinal())
            throw new ArgumentException("Only cardinal directions are allowed.");

        Direction = direction;
        if (position != Point.None)
            PixelPosition = position.SurfaceLocationToPixel(Game.DefaultFontSize);
    }
    public bool Equals(Destination other) =>
        Position == other.Position && Direction == other.Direction;

    public bool Equals(Departure dep) =>
        Position == dep.Position;

    public override bool Equals([NotNullWhen(true)] object? o)
    {
        if (o is Destination d)
            return Equals(d); 
        return false;
    }

    public override int GetHashCode() =>
        Position.GetHashCode() ^ Direction.GetHashCode();

    public static bool operator ==(Destination left, Destination right) =>
        left.Equals(right);

    public static bool operator !=(Destination left, Destination right) =>
        !left.Equals(right);

    public static bool operator ==(Destination left, Departure right) =>
        left.Equals(right);

    public static bool operator !=(Destination left, Departure right) =>
        !left.Equals(right);
}