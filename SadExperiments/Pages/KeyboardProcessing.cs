using System.Collections.ObjectModel;

namespace SadExperiments.Pages;

/*
     * This class visualizes how the SadConsole processes keyboard. Especially interesting is how the KeysPressed collection registers and holds keys.
     * Holding down several keys produces an interesting effect in the Keys Pressed column, where the keys appear to overlap each other
     * (especially noticable when the keys were NOT presses at the same time).
     * 
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

class KeyboardProcessing : Page
{
    const int _xKeysDown = 1,
        _xKeysPressed = 21,
        _xKeysReleased = 41;
    const int ColumnWidth = 10,
        HeaderRowY = 5,
        Gap = (Program.Width - ColumnWidth * 3) / 4,
        Col1X = Gap,
        Col2X = Col1X + ColumnWidth + Gap,
        Col3X = Col2X + ColumnWidth + Gap;
    Rectangle _drawArea;
    Color _headerColor = Color.LightGreen;

    public KeyboardProcessing()
    {
        Title = "Keyboard Processing";
        Summary = "Visualisation of how the SadConsole processes keyboard.";

        Surface.Print(2, "Try pressing and holding single and a few keys at the same time.");

        // print the header for the display of keyboard values
        _drawArea = new(0, HeaderRowY + 1, Width, Height - HeaderRowY);
        Surface.Print(Col1X, HeaderRowY, "Keys Down:", _headerColor);
        Surface.Print(Col2X, HeaderRowY, "Keys Pressed:", _headerColor);
        Surface.Print(Col3X, HeaderRowY, "Keys Released:", _headerColor);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Container)
            IsFocused = true;

        base.OnParentChanged(oldParent, newParent);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        Surface.Clear(_drawArea);
        if (keyboard.HasKeysDown) PrintKeys(keyboard.KeysDown, Col1X);
        if (keyboard.HasKeysPressed) PrintKeys(keyboard.KeysPressed, Col2X);
        if (keyboard.KeysReleased.Count > 0) PrintKeys(keyboard.KeysReleased, Col3X);
        return base.ProcessKeyboard(keyboard);
    }

    void PrintKeys(ReadOnlyCollection<AsciiKey> keys, int x)
    {
        for (var i = 0; i < keys.Count; i++)
        {
            Surface.Print(x, HeaderRowY + 2 + i, keys[i].Key.ToString());
        }
    }
}