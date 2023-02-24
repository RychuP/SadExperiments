using GoRogue.GameFramework;

namespace SadExperiments.Games.PacMan;

abstract class Tile : GameObject
{
    public ColoredGlyph Appearance { get; init; }

    public Tile(ColoredGlyph appearance, Point position, bool isWalkable, bool isTransparent) : 
        base(position, 0, isWalkable, isTransparent)
    {
        Appearance = appearance;
    }
}

class Floor : Tile
{
    public Floor(Point position) : base(Appearances.Floor.Clone(), position, true, true)
    {
        
    }
}

class Wall : Tile
{
    public Wall(Point position) : base(Appearances.Wall.Clone(), position, false, false)
    {
        
    }
}