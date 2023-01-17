using SadConsole.Components;

namespace SadExperiments.Components;

class RandomBackgroundKeyboardComponent : KeyboardConsoleComponent
{
    public override void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
    {
        handled = false;

        if (host is Console c && keyboard.HasKeysPressed && keyboard.IsKeyPressed(Keys.Space))
        {
            c.DefaultBackground = Program.RandomColor;
            c.Clear();
            handled = true;
        }

        // allow changing pages by Container
        if (!handled)
            handled = host.Parent.ProcessKeyboard(keyboard);
    }
}