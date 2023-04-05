namespace SadExperiments.Games.RogueLike.World.Entities;

internal class Corpse : Item
{
    readonly string _description;
    public Corpse(Actor actor) : base('%', actor.Color)
    {
        Position = actor.Position;
        _description = $"Corpse of the {_description}";
    }
    public override string ToString() =>
        _description;
}