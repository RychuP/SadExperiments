namespace SadExperiments.Games.PacMan.Ghosts;

abstract class Ghost : Sprite
{
    #region Fields

    #endregion Fields

    #region Constructors
    public Ghost(Point start) : base(start)
    {
        AnimationSpeed = 0.25d;
        Speed = 1.9d;
    }
    #endregion Constructors

    #region Properties

    #endregion Properties

    #region Methods

    #endregion Methods
}

enum GhostState
{
    Idle,
}