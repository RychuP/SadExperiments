using SadConsole.Entities;

namespace SadExperiments.Games.PacMan;

class Dot : Entity, IEdible
{
    public int Value { get; init; } = 1;

    public Dot(Point position) : base(Appearances.Dot, 0)
    {
        Position = position;
    }
}