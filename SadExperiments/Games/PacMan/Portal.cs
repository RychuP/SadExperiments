namespace SadExperiments.Games.PacMan;

class Portal : Floor
{
    public char Id { get; init; }

    public Portal(Point position, char id) : base(position)
    {
        Id = id;
    }
}