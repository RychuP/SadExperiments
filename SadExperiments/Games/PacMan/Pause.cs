using SadConsole.Instructions;
using Microsoft.Xna.Framework.Audio;

namespace SadExperiments.Games.PacMan;

class Pause : InstructionSet
{
    public Pause(SoundEffectInstance sound, bool stopAllSounds = false)
    {
        RemoveOnFinished = true;
        if (stopAllSounds)
            Sounds.StopAll();
        sound.Play();
        Instructions.AddFirst(
            new CodeInstruction((o, t) => {
                if (sound.State == SoundState.Stopped)
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

class StartPause : Pause
{
    public StartPause() : base(Sounds.Start, true) { }

    public override void OnRemoved(IScreenObject host)
    {
        if (host is Board)
            Sounds.Siren.Play();
        base.OnRemoved(host);
    }
}


class GhostEatenPause : Pause
{
    public GhostEatenPause() : base(Sounds.MunchGhost) { }
}