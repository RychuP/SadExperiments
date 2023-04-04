namespace SadExperiments.Games.RogueLike.World.Entities;

internal class Player : Actor
{
    #region Fields
    public const int FOVRadius = 8;
    #endregion

    #region Constructors
    public Player() : base('@', Color.Yellow, 30, 2, 5)
    {
        
    }
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void Reset() =>
        HP = MaxHP;
    public override string ToString() => "Player";
    #endregion

    #region Event Handlers

    #endregion

    #region Events

    #endregion
}