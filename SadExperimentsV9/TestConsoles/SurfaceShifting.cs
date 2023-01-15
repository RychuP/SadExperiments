using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadExperimentsV9.TestConsoles
{
    internal class SurfaceShifting : TestConsole
    {
        public SurfaceShifting()
        {
            Surface.Fill(glyph: '#');
            IsFocused = true;
        }

        protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
        {
            base.OnParentChanged(oldParent, newParent);
            if (newParent is ScreenSurface s)
                s.Surface.DefaultBackground = Color.DarkBlue;
        }

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            if (keyboard.HasKeysPressed)
            {
                if (keyboard.IsKeyPressed(Keys.Right))
                    Surface.ShiftLeft();
                else if (keyboard.IsKeyPressed(Keys.Left))
                    Surface.ShiftRight();
                else if (keyboard.IsKeyPressed(Keys.Up))
                    Surface.ShiftDown();
                else if (keyboard.IsKeyPressed(Keys.Down))
                    Surface.ShiftUp();
            }
            return base.ProcessKeyboard(keyboard);
        }
    }
}
