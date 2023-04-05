namespace SadExperiments.Games.RogueLike.World.Entities;

internal class Inventory
{
    // in cubic cm
    readonly int _capacity;
    readonly List<ICarryable> _items = new();
    public Inventory(int capacity)
    {
        if (capacity < 1)
            throw new ArgumentException("Minimum capacity is 1.");
        _capacity = capacity;
        _items = new(capacity);
    }
    public IReadOnlyList<ICarryable> Items =>
        _items;
    // total volume this inventory can hold
    public int Capacity =>
        _capacity;
    // total volume of items in this inventory
    public int Volume =>
        _items.Sum(x => x.Volume);
    public bool Remove(ICarryable item) =>
        _items.Remove(item);
    public bool Add(ICarryable item)
    {
        if (Volume + item.Volume > Capacity)
            return false;
        else
        {
            _items.Add(item);
            return true;
        }
    }
}