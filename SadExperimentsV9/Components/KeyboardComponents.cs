using SadConsole.Components;
using SadConsole.Input;

namespace SadExperiments.Components;

class RandomBackgroundKeyboardComponent : KeyboardConsoleComponent
{
    public override void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
    {
        handled = false;

        if (host is Console c && keyboard.HasKeysPressed)
        {
            if (keyboard.IsKeyPressed(Keys.Space))
            {
                c.DefaultBackground = Program.RandomColor;
                c.Clear();
                handled = true;
            }
        }

        // allow changing pages by Container
        host.Parent.ProcessKeyboard(keyboard);
    }
}