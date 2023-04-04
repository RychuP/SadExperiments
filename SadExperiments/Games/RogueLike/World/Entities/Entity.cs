using GoRogue.GameFramework;

namespace SadExperiments.Games.RogueLike.World.Entities;

internal abstract class Entity : GameObject
{
    #region Fields

    #endregion Fields

    #region Constructors
    public Entity(int glyph, Color color, EntityLayer layer, bool IsWalkable, bool IsTransparent) : 
        base((int)layer, IsWalkable, IsTransparent)
    {
        if (layer == EntityLayer.Terrain)
            throw new ArgumentException("Entities cannot by created on terrain layer.");

        (Glyph, Color) = (glyph, color);
    }
    #endregion Constructors

    #region Properties
    public Color Color { get; init; }
    public int Glyph { get; init; }
    #endregion Properties

    #region Events

    #endregion Events
}