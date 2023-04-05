namespace SadExperiments.Games.RogueLike.World.Entities;

internal abstract class Potion : ConsumableItem
{
    public const int DefaultVolume = 200;
    public Potion(string name, int healthAmount, bool isHarmful, Color color, int weight = 150, int volume = 200) 
        : base(name + " Potion", '!', color, healthAmount, 10, weight, isHarmful, DefaultVolume, weight) { }
    public override string EffectDescription =>
        $"drink a {Name.ToLower()} and {(IsHarmful ? "loose" : "recover")} ";
}

internal class HealthPotion : Potion
{
    public HealthPotion() : base("Health", 4, false, Colors.HealingItem) { }
    public override string EffectDescription => 
        base.EffectDescription + $"{HealthAmount} HP";
}