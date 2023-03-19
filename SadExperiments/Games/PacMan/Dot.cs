using SadConsole.Effects;
using SadConsole.Entities;

namespace SadExperiments.Games.PacMan;

class Dot : Entity, IEdible
{
    public int Value { get; init; } = 10;

    public Dot(Point position) : base(Appearances.Dot, 0)
    {
        Position = position;
    }
}

class PowerDot : Dot
{
    public PowerDot(Point position) : base(position)
    {
        Value = 50;
        Appearance = Appearances.PowerDot.Clone();
        Effect = new Blink() { BlinkSpeed = TimeSpan.FromSeconds(1 / 3d) };
    }
}

interface IEdible
{
    int Value { get; init; }
}