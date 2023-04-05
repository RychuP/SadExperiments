namespace SadExperiments.Games.RogueLike.World.Entities;

internal abstract class Item : Entity
{
    public Item(int glyph, Color color) : base(glyph, color, EntityLayer.Items, true, true) { }
}

internal abstract class ConsumableItem : Item, IConsumable
{
    public ConsumableItem(int glyph, Color color, int healthAmount, int foodAmount, 
        int drinkAmount, bool isHarmful) : base(glyph, color) =>
        (HealthAmount, FoodAmount, DrinkAmount, IsHarmful) = (healthAmount, foodAmount, drinkAmount, isHarmful);
    public int HealthAmount { get; init; }
    public int FoodAmount { get; init; }
    public int DrinkAmount { get; init; }
    public bool IsHarmful { get; init; }
    public virtual string EffectDescription =>
        string.Empty;
    public override string ToString() =>
        "Consumable Item";
}