using GoRogue.GameFramework;

namespace SadExperiments.Games.RogueLike.World.Entities;

internal abstract class Entity : GameObject
{
    public Entity(string name, int glyph, Color color, EntityLayer layer, bool IsWalkable, bool IsTransparent) : 
        base((int)layer, IsWalkable, IsTransparent)
    {
        if (layer == EntityLayer.Terrain)
            throw new ArgumentException("Entities cannot by created on terrain layer.");

        (Name, Glyph, Color) = (name, glyph, color);
    }
    public Color Color { get; init; }
    public int Glyph { get; init; }
    public string Name { get; init; }
    public override string ToString() =>
        Name;
}