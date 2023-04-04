using Microsoft.Xna.Framework.Input;

namespace SadExperiments.Games.RogueLike.World.Entities;

internal abstract class Enemy : Actor
{
    public Enemy(int glyph, Color color, int maxHP, int defense, int power) : 
        base(glyph, color, maxHP, defense, power)
    { }
}

internal class Orc : Enemy
{
    static readonly Color LightGreen = new(63, 127, 63);
    public Orc() : base('o', LightGreen, 10, 0, 3) { }
    public override string ToString() => "Orc";
}

internal class Troll : Enemy
{
    static readonly Color DarkGreen = new(0, 127, 0);
    public Troll() : base('T', DarkGreen, 16, 1, 4) { }
    public override string ToString() => "Troll";
}