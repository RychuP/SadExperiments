using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;

namespace SadExperiments.UI
{
    class AutomatedButton : Button
    {
        const int Padding = 4;
        public Keys? KeyboardShortcut { get; init; }
        public bool PrependNameWithKeyboardShortcut { get; set; } = true;

        static AutomatedButton() =>
            Library.Default.SetControlTheme(typeof(AutomatedButton), new ButtonTheme());

        public AutomatedButton(string label, int y, Keys? keyboardShortcut = null) : base(label.Length + Padding, 1)
        {
            KeyboardShortcut = keyboardShortcut;
            Text = label;
            UseMouse = true;
            UseKeyboard = false;
            Position = (0, y);
        }

        /// <summary>
        /// Sets/Gets Text value and automatically resizes/realigns button in the parent.
        /// </summary>
        public new string Text
        {
            get => _text;
            set
            {
                // return if the value is null or empty
                if (string.IsNullOrEmpty(value)) 
                    return;

                // (optional) add keyboard shortcut at the beginning 
                else if (KeyboardShortcut.HasValue && PrependNameWithKeyboardShortcut)
                {
                    string keyboardShortcut = $"{KeyboardShortcut.Value}";
                    if (keyboardShortcut.StartsWith('D') && keyboardShortcut.Length == 2)
                        keyboardShortcut = keyboardShortcut[1..];
                    value = $"{keyboardShortcut}: {value}";
                }

                // check for null case
                _text ??= "";

                // resize and align button
                if (value.Length != _text.Length)
                {
                    Resize(value.Length + Padding, 1);
                    if (Parent is { Host.ParentConsole: { } })
                    {
                        if (Parent.Host.ParentConsole is VerticalButtonsConsole vbc)
                            vbc.AlignButton(this);
                        else if (Parent.Host.ParentConsole is HorizontalButtonsConsole hbc)
                            hbc.AlignButtons(Position.Y);
                    }
                }

                base.Text = value;
            }
        }

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
}
