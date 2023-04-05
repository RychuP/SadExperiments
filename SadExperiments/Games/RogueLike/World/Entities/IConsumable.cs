namespace SadExperiments.Games.RogueLike.World.Entities;

internal interface IConsumable
{
    // in hp points
    int HealthAmount { get; }
    // in calories
    int FoodAmount { get; }
    // in grams
    int DrinkAmount { get; }
    bool IsHarmful { get; }
    string EffectDescription { get; }
}