using SadConsole.Instructions;

namespace SadExperiments.Games.PacMan;

class StartPause : InstructionSet
{
    public StartPause()
    {
        RemoveOnFinished = true;
        Sounds.StopAll();
        Sounds.Start.Play();
        Instructions.AddFirst(
            new CodeInstruction((o, t) => {
                if (Sounds.Start.State == Microsoft.Xna.Framework.Audio.SoundState.Stopped)
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
        {
            board.IsPaused = false;
            board.GhostHouse.StartTimers();
            Sounds.Siren.Play();
        }
        base.OnRemoved(host);
    }
}