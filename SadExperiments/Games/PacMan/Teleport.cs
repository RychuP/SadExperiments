namespace SadExperiments.Games.PacMan;

class Teleport : Floor
{
    public char Identifier { get; init; }

    public Teleport(Point position, char identifier) : base(position)
    {
        Identifier = identifier;
    }
}