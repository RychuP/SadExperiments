using GoRogue.GameFramework;

namespace SadExperiments.Games.RogueLike.World;

internal abstract class Terrain : GameObject
{
    public Color LightColor { get; init; }
    public Color DarkColor { get; init; }

    public Terrain(Point position, Color light, Color dark, bool isWalkable, bool isTransparent) : 
        base(position, (int)EntityLayer.Terrain, isWalkable, isTransparent)
    {
        (LightColor, DarkColor) = (light, dark);
    }
}

class Floor : Terrain
{
    public Floor(Point position) : 
        base(position, new(200, 180, 50), new(50, 50, 150), true, true)
    { }
}

class Wall : Terrain
{
    public Wall(Point position) : 
        base(position, new(130, 110, 50), new(0, 0, 100), false, false)
    { }
}