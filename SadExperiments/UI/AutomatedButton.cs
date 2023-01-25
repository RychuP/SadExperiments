using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;

namespace SadExperiments.UI
{
    class AutomatedButton : Button
    {
        const int Padding = 4;
        public Keys? KeyboardShortcut { get; init; }

        static AutomatedButton() =>
            Library.Default.SetControlTheme(typeof(AutomatedButton), new ButtonTheme());

        public AutomatedButton(string label, int y, Keys? keyboardShortcut = null) : base(label.Length + Padding, 1)
        {
            Text = label;
            UseMouse = true;
            UseKeyboard = false;
            KeyboardShortcut = keyboardShortcut;
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
                if (string.IsNullOrEmpty(value)) return;
                if (!string.IsNullOrEmpty(_text) && value.Length != _text.Length)
                {
                    Resize(value.Length + Padding, 1);
                    if (Parent is VerticalButtonsConsole vbc)
                        vbc.AlignButton(this);
                    else if (Parent is HorizontalButtonsConsole hbc)
                        hbc.AlignButtons(Position.Y);
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
