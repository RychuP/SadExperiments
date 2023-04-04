using GoRogue.GameFramework;

namespace SadExperiments.Games.RogueLike.World.Entities;

internal abstract class Actor : Entity
{
    int _hp;

    public Actor(int glyph, Color color, int maxHP, int defense, int power) 
        : base(glyph, color, EntityLayer.Actors, false, true)
    {
        (MaxHP, _hp, Defense, Power) = (maxHP, maxHP, defense, power);
    }

    public int MaxHP { get; init; }
    public int Defense { get; init; }
    public int Power { get; init; }

    public int HP
    {
        get => _hp;
        protected set
        {
            if (_hp == value) return;

            _hp = Math.Min(Math.Max(0, value), MaxHP);
            HPChanged?.Invoke(this, EventArgs.Empty);

            if (_hp == 0)
                Died?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Attack(Actor other)
    {
        int damage = Power - other.Defense;
        OnAttacked(damage, other);
        if (damage > 0)
            other.HP -= damage;
    }

    void OnAttacked(int damage, Actor other)
    {
        var args = new CombatEventArgs(damage, other);
        Attacked?.Invoke(this, args);
    }

    public event EventHandler<CombatEventArgs>? Attacked;
    public event EventHandler? HPChanged;
    public event EventHandler? Died;
}