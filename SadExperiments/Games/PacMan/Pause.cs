using SadConsole.Instructions;

namespace SadExperiments.Games.PacMan;

class Pause : InstructionSet
{
    public Pause()
    {
        RemoveOnFinished = true;
        Instructions.AddFirst(new Wait(TimeSpan.FromSeconds(1)));
        Instructions.AddLast(new CodeInstruction((o, t) =>
        {
            if (o is Board b) b.TogglePause();
            return true;
        }));
    }
}