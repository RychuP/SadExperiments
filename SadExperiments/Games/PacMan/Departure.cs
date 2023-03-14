using System.Diagnostics.CodeAnalysis;

namespace SadExperiments.Games.PacMan;

// sprite departure position
readonly struct Departure : IEquatable<Departure>
{
    public static readonly Departure None = new();
    public Point Position { get; init; }
    public Point PixelPosition { get; init; } = Point.None;

    public Departure() =>
        Position = Point.None;

    public Departure(Point position)
    {
        Position = position;
        if (position != Point.None)
            PixelPosition = position.SurfaceLocationToPixel(Game.DefaultFontSize);
    }

    public bool Equals(Departure other) =>
        Position == other.Position;

    public bool Eqauls(Destination dest) =>
        Position == dest.Position;

    public override bool Equals([NotNullWhen(true)] object? o)
    {
        if (o is Departure d)
            return Equals(d);
        return false;
    }

    public override int GetHashCode() => 
        Position.GetHashCode();

    public static bool operator ==(Departure left, Departure right) => 
        left.Equals(right);
    public static bool operator !=(Departure left, Departure right) => 
        !left.Equals(right);

    public static bool operator ==(Departure left, Destination right) =>
        left.Equals(right);

    public static bool operator !=(Departure left, Destination right) =>
        !left.Equals(right);
}