using SadConsole.UI.Controls;

namespace SadExperiments.UI.Controls;

class DotButton : RadioButton
{
    public DotButton() : base(1, 1)
    {
        CanFocus = false;
    }

    public override void DetermineState()
    {
        if (IsSelected)
            State |= ControlStates.Selected;
        else
            State &= ~ControlStates.Selected;
        base.DetermineState();
    }
}
