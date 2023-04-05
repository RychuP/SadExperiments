namespace SadExperiments.Games.RogueLike.World.Entities;

internal abstract class Actor : Entity
{
    int _hp;

    public Actor(string name, int glyph, Color color, int maxHP, int defense, int power, int capacity) 
        : base(name, glyph, color, EntityLayer.Actors, false, true)
    {
        (MaxHP, _hp, Defense, Power) = (maxHP, maxHP, defense, power);
        Inventory = new(capacity);
    }

    public int MaxHP { get; init; }
    public int Defense { get; init; }
    public int Power { get; init; }
    public Inventory Inventory { get; init; }

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

    public bool IsAlive() =>
        HP > 0;

    public void Attack(Actor other)
    {
        int damage = Power - other.Defense;
        OnAttacked(damage, other);
        if (damage > 0)
            other.HP -= damage;
    }

    public void Consume(IConsumable consumable)
    {
        HP += consumable.IsHarmful ? -consumable.HealthAmount : consumable.HealthAmount;
        OnConsumed(consumable);
    }

    public bool TryCollect(ICarryable item)
    {
        if (Inventory.Add(item))
        {
            OnCollected(item);
            return true;
        }
        else
        {
            string message = $"Not enough capacity to fit the {item.ToString()?.ToLower()} in.";
            OnFailedAction(message);
            return false;
        }
    }

    protected void OnFailedAction(string message)
    {
        var args = new FailedActionEventArgs(message);
        FailedAction?.Invoke(this, args);
    }

    void OnCollected(ICarryable item)
    {
        var args = new ItemEventArgs(item);
        Collected?.Invoke(this, args);
    }

    void OnConsumed(IConsumable item)
    {
        var args = new ConsumedEventArgs(item);
        Consumed?.Invoke(this, args);
    }

    void OnAttacked(int damage, Actor other)
    {
        var args = new CombatEventArgs(damage, other);
        Attacked?.Invoke(this, args);
    }

    public event EventHandler<CombatEventArgs>? Attacked;
    public event EventHandler<ConsumedEventArgs>? Consumed;
    public event EventHandler<ItemEventArgs>? Collected;
    public event EventHandler<FailedActionEventArgs>? FailedAction;
    public event EventHandler? HPChanged;
    public event EventHandler? Died;
}