namespace SadExperiments.Games.RogueLike.World.Entities;

internal abstract class Item : Entity
{
    public Item(string name, int glyph, Color color) : 
        base(name, glyph, color, EntityLayer.Items, true, true) { }
}

internal abstract class ConsumableItem : Item, IConsumable, ICarryable
{
    public ConsumableItem(string name, int glyph, Color color, int healthAmount, int foodAmount, 
        int drinkAmount, bool isHarmful, int volume, int weight) : 
        base(name, glyph, color) =>
        (HealthAmount, FoodAmount, DrinkAmount, IsHarmful, Volume, Weight) = 
        (healthAmount, foodAmount, drinkAmount, isHarmful, volume, weight);
    public int HealthAmount { get; init; }
    public int FoodAmount { get; init; }
    public int DrinkAmount { get; init; }
    public bool IsHarmful { get; init; }
    public int Volume { get; init; }
    public int Weight { get; init; }
    public virtual string EffectDescription =>
        string.Empty;
}