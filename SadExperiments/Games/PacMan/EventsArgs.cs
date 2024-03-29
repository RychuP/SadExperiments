using SadExperiments.Games.PacMan.Ghosts;

namespace SadExperiments.Games.PacMan;

class DotEventArgs : EventArgs
{
    public Dot Dot { get; init; }
    public DotEventArgs(Dot dot) => Dot = dot;
}

class GhostEventArgs : EventArgs
{
    public int Value { get; init; }
    public Ghost Ghost { get; init; }
    public GhostEventArgs(Ghost ghost, int value)
    {
        Ghost = ghost;
        Value = value;
    }
}

class GhostModeEventArgs : EventArgs
{
    public GhostMode PrevMode { get; init; }
    public GhostMode NewMode { get; init; }
    public GhostModeEventArgs(GhostMode prevMode, GhostMode newMode) =>
        (PrevMode, NewMode) = (prevMode, newMode);
}