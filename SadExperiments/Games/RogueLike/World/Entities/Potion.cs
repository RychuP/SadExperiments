namespace SadExperiments.Games.RogueLike.World.Entities;

internal abstract class Potion : ConsumableItem
{
    public const int DefaultVolume = 200;
    public Potion(int healthAmount, bool isHarmful, Color color, int weight = 150, int volume = 200) 
        : base('!', color, healthAmount, 10, weight, isHarmful, DefaultVolume, weight) { }
    public override string EffectDescription =>
        $"drinks {ToString().ToLower()} and {(IsHarmful ? "looses" : "recovers")} ";
    public override string ToString() =>
        "Potion";
}

internal class HealthPotion : Potion
{
    public HealthPotion() : base(4, false, Colors.HealingItem) { }
    public override string EffectDescription => 
        base.EffectDescription + $"{HealthAmount} HP.";
    public override string ToString() =>
        "Heal Potion";
}