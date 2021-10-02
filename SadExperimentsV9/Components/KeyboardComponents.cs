using System;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace SadExperiments
{
    class RandomBackgroundKeyboardComponent : KeyboardConsoleComponent
    {
        public override void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
        {
            if (host is Console c && keyboard.HasKeysPressed)
            {
                if (keyboard.IsKeyPressed(Keys.Space))
                {
                    c.DefaultBackground = Program.RandomColor;
                    c.Clear();
                }
                handled = true;
            }
            else
            {
                handled = false;
            }
        }
    }
}
