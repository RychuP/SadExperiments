using GoRogue.GameFramework;
using GoRogue.SpatialMaps;
using SadExperiments.Games.RogueLike.World.Entities;

namespace SadExperiments.Games.RogueLike;

class CombatEventArgs : EventArgs
{
    public int Damage { get; init; }
    public Actor Target { get; init; }
    public CombatEventArgs(int damage, Actor target)
    {
        Damage = damage;
        Target = target;
    }
}

class ConsumedEventArgs : EventArgs
{
    public ConsumableItem Item { get; init; }
    public ConsumedEventArgs(ConsumableItem item) =>
        Item = item;
}

class MapGeneratedEventArgs : EventArgs
{
    public IReadOnlySpatialMap<IGameObject> Actors { get; init; }
    public MapGeneratedEventArgs(IReadOnlySpatialMap<IGameObject> actors)
    {
        Actors = actors;
    }
}