namespace SadExperiments.Games.PacMan;

class PowerUp : Dot
{
    public PowerUp(Point position) : base(position)
    {
        Value = 2;
        Appearance.Glyph = 37;
    }
}