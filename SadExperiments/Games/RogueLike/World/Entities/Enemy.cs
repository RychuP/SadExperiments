namespace SadExperiments.Games.RogueLike.World.Entities;

internal abstract class Enemy : Actor
{
    public Enemy(string name, int glyph, Color color, int maxHP, int defense, int power, int invCapacity) : 
        base(name, glyph, color, maxHP, defense, power, invCapacity)
    { }
}

internal class Orc : Enemy
{
    static readonly Color LightGreen = new(63, 127, 63);
    public Orc() : base("Orc", 'o', LightGreen, 10, 0, 3, Potion.DefaultVolume * 2) { }
}

internal class Troll : Enemy
{
    static readonly Color DarkGreen = new(0, 127, 0);
    public Troll() : base("Troll", 'T', DarkGreen, 16, 1, 4, Potion.DefaultVolume * 3) { }
}