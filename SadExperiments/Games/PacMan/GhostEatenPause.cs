using SadConsole.Instructions;

namespace SadExperiments.Games.PacMan;

class GhostEatenPause : InstructionSet
{
    public GhostEatenPause()
    {
        RemoveOnFinished = true;
        Sounds.MunchGhost.Play();
        Instructions.AddFirst(
            new CodeInstruction((o, t) => {
                if (Sounds.MunchGhost.State == Microsoft.Xna.Framework.Audio.SoundState.Stopped)
                    return true;
                return false;
            })
        );
    }

    public override void OnAdded(IScreenObject host)
    {
        if (host is Board board)
            board.IsPaused = true;
        base.OnAdded(host);
    }

    public override void OnRemoved(IScreenObject host)
    {
        if (host is Board board)
            board.IsPaused = false;
        base.OnRemoved(host);
    }
}