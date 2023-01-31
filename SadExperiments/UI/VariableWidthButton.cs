using SadConsole.UI.Controls;
using SadConsole.UI.Themes;

namespace SadExperiments.UI;

class VariableWidthButton : Button, IHasKeyboardShortcut
{
    const int Padding = 4;
    public Keys? KeyboardShortcut { get; init; }
    public bool AddKeyboardShortcutToText { get; set; }

    static VariableWidthButton() =>
        Library.Default.SetControlTheme(typeof(VariableWidthButton), new ButtonTheme());

    public VariableWidthButton(string text, Keys? keyboardShortcut = null, bool addKeyboardShortcutToText = true) : 
        base(text.Length + Padding, 1)
    {
        AddKeyboardShortcutToText = addKeyboardShortcutToText;
        KeyboardShortcut = keyboardShortcut;
        Text = text;
        UseMouse = true;
        UseKeyboard = false;
    }

    /// <summary>
    /// Sets/Gets Text value and automatically resizes/realigns button in the parent.
    /// </summary>
    public new string Text
    {
        get => _text;
        set
        {
            if (string.IsNullOrEmpty(value))
                return;

            // add a keyboard shortcut name at the beginning of button text
            else if (KeyboardShortcut.HasValue && AddKeyboardShortcutToText)
            {
                string keyboardShortcut = $"{KeyboardShortcut.Value}";
                if (keyboardShortcut.StartsWith('D') && keyboardShortcut.Length == 2)
                    keyboardShortcut = keyboardShortcut[1..];
                value = $"{keyboardShortcut}: {value}";
            }

            // resize button if required
            if (value.Length + Padding != Width)
                Resize(value.Length + Padding, 1);

            base.Text = value;
        }
    }

    public event EventHandler<ValueChangedEventArgs<int>>? WidthChanged;

    public override void Resize(int width, int height)
    {
        var args = new ValueChangedEventArgs<int>(Width, width);
        base.Resize(width, height);
        OnWidthChanged(args);
    }
    
    protected virtual void OnWidthChanged(ValueChangedEventArgs<int> args) =>
        WidthChanged?.Invoke(this, args);

    public new void InvokeClick()
    {
        // Fancy check to make sure Parent, Parent.Host, and Parent.Host.ParentConsole are all non-null
        if (Parent is { Host.ParentConsole: { } })
            Parent.Host.ParentConsole.IsFocused = true;

        IsFocused = true;
        base.InvokeClick();
        DetermineState();
    }
}

public interface IHasKeyboardShortcut
{
    Keys? KeyboardShortcut { get; }
}