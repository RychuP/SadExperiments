namespace SadExperiments.Games.RogueLike.World.Entities;

internal class Player : Actor
{
    #region Fields
    public const int FOVRadius = 8;
    #endregion

    #region Constructors
    public Player() : base("Player", '@', Color.Yellow, 30, 2, 5, Potion.DefaultVolume * 6) { }
    #endregion

    #region Methods
    public void Reset() =>
        HP = MaxHP;
    
    public void TryConsumeHealthPotion()
    {
        var potion = Inventory.Items.Where(o => o is HealthPotion p).Cast<HealthPotion>().FirstOrDefault();
        if (potion is not null)
        {
            Inventory.Remove(potion);
            Consume(potion);
        }
        else
            OnFailedAction("You don't have any health potions.");
    }
    #endregion
}