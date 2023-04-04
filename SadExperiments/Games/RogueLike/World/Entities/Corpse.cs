namespace SadExperiments.Games.RogueLike.World.Entities;

internal class Corpse : Entity
{
    readonly string _description;
    public Corpse(Enemy enemy) : base('%', enemy.Color, EntityLayer.Items, true, true)
    {
        Position = enemy.Position;
        _description = $"Corpse of {_description}";
    }
    public override string ToString() =>
        _description;
}