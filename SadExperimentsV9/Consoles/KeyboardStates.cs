using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadConsole;
using SadConsole.Input;
using SadConsole.Entities;
using System.Collections;
using System.Collections.ObjectModel;
using SadRogue.Primitives;

namespace SadExperimentsV9.Consoles
{
    class KeyboardStates : ScreenSurface
    {
        int _xKeysDown = 1,
            _xKeysPressed = 21,
            _xKeysReleased = 41;
        Rectangle _drawArea;

        public KeyboardStates(int w, int h) : base (w, h)
        {
            _drawArea = new(0, 2, w, h - 2);
            Surface.Print(_xKeysDown, 1, "Keys Down:");
            Surface.Print(_xKeysPressed, 1, "Keys Pressed:");
            Surface.Print(_xKeysReleased, 1, "Keys Released:");
            IsFocused = true;
        }

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            Surface.Clear(_drawArea);
            if (keyboard.HasKeysDown)
            {
                PrintKeys(keyboard.KeysDown, _xKeysDown);
            }
            if (keyboard.HasKeysPressed) PrintKeys(keyboard.KeysPressed, _xKeysPressed);
            if (keyboard.KeysReleased.Count > 0) PrintKeys(keyboard.KeysReleased, _xKeysReleased);
            return true;
        }

        void PrintKeys(ReadOnlyCollection<AsciiKey> keys, int x)
        {
            for (var i = 0; i < keys.Count; i++)
            {
                Surface.Print(x, 3 + i, keys[i].Key.ToString());
            }
        }
    }
}
