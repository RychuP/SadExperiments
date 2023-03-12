using SadConsole.Instructions;

namespace SadExperiments.Games.PacMan;

class Pause : InstructionSet
{
    readonly Action? _callback;

    public Pause(double duration = 1d, Action? callback = null)
    {
        _callback = callback;
        RemoveOnFinished = true;
        Instructions.AddFirst(new Wait(TimeSpan.FromSeconds(duration)));
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
            _callback?.Invoke();
        }
        base.OnRemoved(host);
    }
}