using SadConsole.Instructions;

namespace SadExperiments.Games.PacMan;

class Pause : InstructionSet
{
    public Pause(double duration = 1d)
    {
        RemoveOnFinished = true;
        Instructions.AddFirst(new Wait(TimeSpan.FromSeconds(duration)));
        Instructions.AddLast(new CodeInstruction((o, t) =>
        {
            if (o is Board b) b.TogglePause();
            return true;
        }));
    }
}