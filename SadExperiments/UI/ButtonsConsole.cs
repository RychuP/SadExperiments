using SadConsole.Quick;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadExperiments.UI.Controls;

namespace SadExperiments.UI;

class HorizontalButtonsConsole : ButtonsConsole
{
    public HorizontalButtonsConsole(int w, int h) : base(w, h) { }

    public override VariableWidthButton AddButton(string text, Keys? keyboardShortcut = null, bool addKeyboardShortcutToText = true) =>
        AddButton(text, (0, CurrentRow), keyboardShortcut, addKeyboardShortcutToText);

    public override void AlignRow(VariableWidthButton button) =>
        AlignRow(button.Position.Y);

    public override void AlignRow(int y)
    {
        // compute space between buttons for the row
        var buttons = Controls.Where(control => control.Position.Y == y).ToArray();
        int allButtonsInRowWidth = buttons.Sum(control => control.Width);
        int buttonCount = buttons.Length;
        double spacer = (double)(Width - allButtonsInRowWidth) / (buttonCount + 1);

        // overflow case
        if (spacer < 1)
        {
            // move the last added button to a new row and advance _currentRow pointer
            CurrentRow += VerticalSpacing + 1;
            var lastButton = Controls[Controls.Count - 1];
            int column = Width / 2 - lastButton.Width / 2;
            lastButton.Position = (column, CurrentRow);

            // resize surface if needed
            if (Height < CurrentRow + 1)
                Resize(Width, CurrentRow + 1, Width, CurrentRow + 1, false);

            return;
        }

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
        Point position = (0, Controls.Count > 0 ? Controls.Count + VerticalSpacing * Controls.Count : 0);
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

// base class for button hosts 
abstract class ButtonsConsole : ControlsConsole
{
    /// <summary>
    /// Spacing between the rows of buttons.
    /// </summary>
    public int VerticalSpacing { get; set; } = 1;

    /// <summary>
    /// Current row to which the buttons are added automatically.
    /// </summary>
    public int CurrentRow { get; set; } = 0;

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

    /// <summary>
    /// Removes all buttons, clears the console and resizes its height to 1.
    /// </summary>
    public void RemoveAll()
    {
        Controls.Clear();
        CurrentRow = 0;
        Resize(Width, 1, Width, 1, true);
    }

    protected virtual void OnButtonWidthChanged(object? o, ValueChangedEventArgs<int> e)
    {
        if (o is VariableWidthButton button)
            AlignRow(button);
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
    // extracted from the ProcessKeyboard() to allow other objects
    // that hold focus to check this console for keyboard shortcuts
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