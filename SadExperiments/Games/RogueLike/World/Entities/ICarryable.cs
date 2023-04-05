namespace SadExperiments.Games.RogueLike.World.Entities;

internal interface ICarryable
{
    // in cubic centimeters
    int Volume { get; }
    // in grams
    int Weight { get; }
    string Name { get; }
}