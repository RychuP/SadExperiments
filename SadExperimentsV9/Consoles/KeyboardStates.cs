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
    /*
     * Chris3606 Explains:
     * 
     * ... an interesting (and as far as I know, intended) characteristic of that system; 
     * each key is repeated based on two values; an initial repeat delay, and a second repeat delay that applies every subsequent time after the initial delay.  
     * And that delay is tracked independently for each key.  
     * So If you happen to press the keys in such a sequence that the second repeat delays are offset from each other, only one is in the list at once.
     * 
     * There's only one KeysPressed collection.  According to my understanding, a given key is in that single KeysPressed collection if, and only if:
        1. It has just been pressed as of the frame currently being processed.  It is removed from the collection on the very next frame
        2. It has been in the "pressed" collection before, but never repeated; and the amount of time listed in the Keyboard.InitialRepeat property has passed between the last frame in which it was in the collection, and the current one.  It will be in the "pressed" collection for this frame, and removed from the collection on the very next frame
        3. It has been in the "pressed" collection before, and repeated at least once; and the amount of time listed in Keyboard.RepeatDelay property has passed between the last frame in which it was in the collection, and the current one.  It will be in the "pressed" collection for this frame, and removed from the collection on the very next frame 
    * so if we have initial repeat delay of 0.8, and subsequent repeat delay of 0.04.
    * Then "A" is physically pressed and held down at the keyboard starting at time unit 0.  It will be in the keys pressed collection at times [0, 0.8, 0.84, 0.88, 0.92, 0.96 ...] until it is released; thus perfectly interleaving them such that 1 and only 1 key is in the pressed collection at a time.
    * If, while "A" is being held down, "B" is physically pressed and held down starting at time unit 0.1, it will be in the keys pressed collection at times [0.1, 0.9, 0.94, 0.98, ...] until it is released 
    * Whereas, if both keys were first physically pressed and held at time 0, then they are both always in the collection at the exact same time (until, of course, one of them is released) 
    *
    */

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
