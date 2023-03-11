using SadConsole.Effects;

namespace SadExperiments.Games.PacMan;

class PowerDot : Dot
{
    public PowerDot(Point position) : base(position)
    {
        Value = 5;
        Appearance = Appearances.PowerDot.Clone();
        Effect = new Blink() { BlinkSpeed = TimeSpan.FromSeconds(1 / 3d) };
    }
}