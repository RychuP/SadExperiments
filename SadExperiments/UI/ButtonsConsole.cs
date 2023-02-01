using SadConsole.Quick;
using SadConsole.UI;

namespace SadExperiments.UI;

class HorizontalButtonsConsole : ButtonsConsole
{
    public HorizontalButtonsConsole(int w, int h) : base(w, h) { }

    public override VariableWidthButton AddButton(string text, Keys? keyboardShortcut = null, bool addKeyboardShortcutToText = true) =>
        AddButton(text, (0, 0), keyboardShortcut, addKeyboardShortcutToText);

    public override void AlignRow(VariableWidthButton button) =>
        AlignRow(button.Position.Y);

    public override void AlignRow(int y)
    {
        // compute space between buttons for the row
        var buttons = Controls.Where(control => control.Position.Y == y).ToArray();
        int allButtonsInRowWidth = buttons.Sum(control => control.Width);
        int buttonCount = buttons.Length;
        double spacer = (double)(Width - allButtonsInRowWidth) / (buttonCount + 1);
        if (spacer < 0) spacer = 0;

        // apply new positions
        double x = spacer;
        for (int i = 0; i < buttonCount; i++)
        {
            var control = buttons[i];
            control.Position = control.Position.WithX((int)Math.Round(x));
            x += control.Width + spacer;
        }
    }
}

class VerticalButtonsConsole : ButtonsConsole
{
    public VerticalButtonsConsole(int w, int h) : base(w, h) { }

    public override VariableWidthButton AddButton(string text, Keys? keyboardShortcut = null, bool addKeyboardShortcutToText = true)
    {
        Point position = (0, Controls.Count > 0 ? Controls.Count + VerticalSpacing * (Controls.Count) : 0);
        return AddButton(text, position, keyboardShortcut, addKeyboardShortcutToText);
    }

    public override void AlignRow(VariableWidthButton button) =>
        button.Position = button.Position.WithX(Surface.Width / 2 - button.Width / 2);

    public override void AlignRow(int y)
    {
        var screenObject = Children.Where(o => o.Position.Y == y).FirstOrDefault();
        if (screenObject is VariableWidthButton button) 
            AlignRow(button); 
    }
}

// base class for button hosts (automatically adds handling of global page keyboard handling)
abstract class ButtonsConsole : ControlsConsole
{
    /// <summary>
    /// Spacing between the rows of buttons.
    /// </summary>
    public int VerticalSpacing { get; set; } = 1;

    public ButtonsConsole(int w, int h) : base(w, h)
    {
        DefaultBackground = Color.Transparent;
        Surface.Clear();
    }

    protected VariableWidthButton AddButton(string text, Point position, Keys? keyboardShortcut = null,
        bool addKeyboardShortcutToText = true)
    {
        var button = new VariableWidthButton(text, keyboardShortcut, addKeyboardShortcutToText);
        button.Position = position;
        button.WidthChanged += OnButtonWidthChanged;
        Controls.Add(button);
        AlignRow(button);
        return button;
    }

    protected virtual void OnButtonWidthChanged(object? o, ValueChangedEventArgs<int> e)
    {
        if (o is VariableWidthButton button)
            AlignRow(button);
    }

    // adds processing of page.ProcessKeyboard() when the buttons are pressed and IsFocused changes to this console
    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Page page)
            this.WithKeyboard((o, k) => page.ProcessKeyboard(k));
        base.OnParentChanged(oldParent, newParent);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (KeyboardShortcutPressed(keyboard))
            return true;
        return base.ProcessKeyboard(keyboard);
    }

    // searches for buttons with keyboard shortcuts and invokes click on them
    // if the corresponding button was pressed
    //
    // this method is extracted from the ProcessKeyboard() to prevent infinite loops when a page holding 
    // the buttons console has more than one of these and checks them all in their ProcessKeyboard()
    // when they are not focused
    //
    // infinite loops mentioned above may result from the keyboard component added in
    // OnParentChanged() that calls page.ProcessKeyboard()
    public bool KeyboardShortcutPressed(Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed)
        {
            foreach (var control in Controls)
            {
                if (control is VariableWidthButton button
                    && button.KeyboardShortcut.HasValue
                    && keyboard.IsKeyPressed(button.KeyboardShortcut.Value))
                {
                    button.InvokeClick();
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Creates a new <see cref="VariableWidthButton"/>, adds it to the <see cref="ControlHost"/> and aligns it according to its width.
    /// </summary>
    /// <param name="text">Label that will be assigned to the Text property.</param>
    /// <param name="keyboardShortcut">Optional keyboard shortcut.</param>
    /// <param name="addKeyboardShortcutToText">If true the keyboard shortcut will appear in the text of the button.</param>
    /// <returns>New <see cref="VariableWidthButton"/>.</returns>
    public abstract VariableWidthButton AddButton(string text, Keys? keyboardShortcut = null, bool addKeyboardShortcutToText = true);

    public abstract void AlignRow(VariableWidthButton button);

    public abstract void AlignRow(int y);
}