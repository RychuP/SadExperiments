using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadExperiments.UI
{
    class HorizontalButtonsConsole : ButtonsConsole
    {
        public HorizontalButtonsConsole(int w, int h) : base(w, h) { }

        /// <summary>
        /// Creates a new <see cref="AutomatedButton"/> and adds it to the <see cref="ControlHost"/>.
        /// </summary>
        /// <param name="label">Label that will be assigned to the Text property.</param>
        /// <param name="y">Y coordinate of the button's Position.</param>
        /// <param name="keyboardShortcut">Optional keyboard shortcut.</param>
        /// <returns>New <see cref="AutomatedButton"/>.</returns>
        public AutomatedButton AddButton(string label, int y, Keys? keyboardShortcut = null)
        {
            var button = new AutomatedButton(label, y, keyboardShortcut);
            Controls.Add(button);
            AlignButtons(y);
            return button;
        }

        public void AlignButtons(int y)
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
        /// <summary>
        /// Number of rows between vertical buttons
        /// </summary>
        public int VerticalSpacing { get; set; } = 1;

        public VerticalButtonsConsole(int w, int h) : base(w, h) { }

        /// <summary>
        /// Creates a new <see cref="AutomatedButton"/> and adds it to the <see cref="ControlHost"/>.
        /// </summary>
        /// <param name="label">Label that will be assigned to the Text property.</param>
        /// <param name="keyboardShortcut">Optional keyboard shortcut.</param>
        /// <returns>New <see cref="AutomatedButton"/>.</returns>
        public AutomatedButton AddButton(string label, Keys? keyboardShortcut = null)
        {
            int y = Controls.Count > 0 ? Controls.Last().Position.Y : 0;
            var button = new AutomatedButton(label, y, keyboardShortcut);
            AlignButton(button);
            return button;
        }

        public void AlignButton(AutomatedButton button) =>
            button.Position = button.Position.WithX(Surface.Width / 2 - button.Width / 2);

    }

    class ButtonsConsole : ControlsConsole
    {
        public ButtonsConsole(int w, int h) : base(w, h)
        {
            DefaultBackground = Color.Transparent;
            Surface.Clear();
        }
    }
}
