namespace SadExperiments.Games.RogueLike.World.Entities;

internal class Corpse : Item
{
    public Corpse(Actor actor) : base($"Corpse of the {actor.Name}", '%', actor.Color)
    {
        Position = actor.Position;
    }
}