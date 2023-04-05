using GoRogue.GameFramework;
using GoRogue.SpatialMaps;
using SadExperiments.Games.RogueLike.World.Entities;

namespace SadExperiments.Games.RogueLike;

class ItemEventArgs : EventArgs
{
    public ICarryable Item { get; init; }
    public ItemEventArgs(ICarryable item) =>
        Item = item;
}

class FailedActionEventArgs : EventArgs
{
    public string Message { get; init; }
    public FailedActionEventArgs(string message) =>
        Message = message;
}

class CombatEventArgs : EventArgs
{
    public int Damage { get; init; }
    public Actor Target { get; init; }
    public CombatEventArgs(int damage, Actor target) =>
        (Damage, Target) = (damage, target);
}

class ConsumedEventArgs : EventArgs
{
    public IConsumable Item { get; init; }
    public ConsumedEventArgs(IConsumable item) =>
        Item = item;
}

class MapGeneratedEventArgs : EventArgs
{
    public IReadOnlySpatialMap<IGameObject> Actors { get; init; }
    public MapGeneratedEventArgs(IReadOnlySpatialMap<IGameObject> actors) =>
        Actors = actors;
}