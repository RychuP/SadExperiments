namespace SadExperiments.Games.RogueLike.World.Entities;

internal abstract class Potion : ConsumableItem
{
    public Potion(int healthAmount, int foodAmount, int drinkAmount, bool isHarmful, Color color) 
        : base('!', color, healthAmount, foodAmount, drinkAmount, isHarmful) { }
    public override string EffectDescription =>
        $"drinks {this.ToString().ToLower()} and {(IsHarmful ? "looses" : "recovers")} ";
    public override string ToString() =>
        "Potion";
}

internal class HealthPotion : Potion
{
    public HealthPotion() : base(4, 0, 1, false, Colors.HealingItem) { }
    public override string EffectDescription => 
        base.EffectDescription + "HP.";
    public override string ToString() =>
        "Heal Potion";
}