namespace SadExperiments.Games.RogueLike.World.Entities;

internal interface IConsumable
{
    int HealthAmount { get; }
    int FoodAmount { get; }
    int DrinkAmount { get; }
    bool IsHarmful { get; }
    string EffectDescription { get; }
}