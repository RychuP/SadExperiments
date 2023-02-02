using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadExperiments.MainScreen;

internal class ContentsList : Page
{
    Filters _filtersConsole;

    public ContentsList(ControlsConsole contentsList)
    {
        Title = "List of Contents";
        Summary = "Select a page to show.";
        //Children.Add(contentsList);
        IsVisible = false;

        var buttons = new SubPage(Width, Height - 5)
        {
            Parent = this,
            Position = new Point(0, 5),
        };
        SubPage.Surface.SetDefaultColors(Color.White, Color.Black);
        Surface.SetDefaultColors(Color.White, Color.Black);

        // create filters console
        _filtersConsole = new Filters()
        {
            Parent = this,
        };
    }

    public void Show()
    {
        IsVisible = true;
        IsFocused = true;
    }

    public void Hide()
    {
        IsVisible = false;
        _filtersConsole.Hide();
    }

    class Filters : ControlsConsole
    {
        const int MinimizedHeight = 4;
        readonly TextBox _tag1TextBox;
        readonly TextBox _tag2TextBox;
        readonly TextBox _sortOrderTextBox;
        readonly ControlsConsole _tag1Buttons;
        readonly ControlsConsole _tag2Buttons;
        readonly ControlsConsole _sortOrderButtons;
        readonly Color BG = Color.AnsiBlackBright;
        readonly Color FG = Color.White;

        public Filters() : base(Program.Width, 4, Program.Width, Program.Height / 2) 
        {
            Surface.SetDefaultColors(FG, BG);

            // calculate controls dimensions
            int noOfControls = 3;
            int spacer = 4;
            int controlWidth = (Width - noOfControls * spacer) / noOfControls;
            int x = spacer;

            // create filter text boxes
            _tag1TextBox = CreateTextBox("Filter by 1st tag:", controlWidth, x);
            _tag2TextBox = CreateTextBox("Filter by 2nd tag:", controlWidth, x += controlWidth + spacer);
            _sortOrderTextBox = CreateTextBox("Sort method:", controlWidth, x += controlWidth + spacer);

            // calculate controls dimensions
            x = spacer;
            int y = 3;
            int controlHeight = Height - y;

            // create button consoles for tag / sort order selection
            _tag1Buttons = CreateButtonsConsole((x, y), controlWidth, controlHeight);
            _tag2Buttons = CreateButtonsConsole((x += controlWidth + spacer, y), controlWidth, controlHeight);
            _sortOrderButtons = CreateButtonsConsole((x += controlWidth + spacer, y), controlWidth, controlHeight);
        }

        ControlsConsole CreateButtonsConsole(Point position, int width, int height)
        {
            var buttons = new ControlsConsole(width, height)
            {
                Parent = this,
                Position = position,
                IsVisible = false,
            };
            buttons.Surface.SetDefaultColors(FG, BG);
            buttons.Surface.DrawOutline();
            return buttons;
        }

        TextBox CreateTextBox(string title, int width, int x)
        {
            Surface.Print(x, 1, title);
            var textBox = new TextBox(width);
            Controls.Add(textBox);
            textBox.Position = (x, 2);
            textBox.EditModeEnter += TextBox_OnEditModeEnter;
            textBox.EditModeExit += TextBox_OnEditModeExit;
            return textBox;
        }

        public void Hide()
        {
            if (!IsMinimized)
                Minimize();
            foreach (var control in Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.IsFocused = false;
                }
            }
        }

        public bool IsMinimized =>
            Surface.ViewHeight == MinimizedHeight;

        /// <summary>
        /// Collapses top section of the console and hides tag selection buttons.
        /// </summary>
        public void Minimize()
        {
            Surface.ViewHeight = MinimizedHeight;
            foreach (var child in Children)
            {
                if (child is ControlsConsole console)
                {
                    console.IsVisible = false;
                }
            }
        }

        /// <summary>
        /// Expands top section of the console to show tag selection buttons.
        /// </summary>
        public void Maximize()
        {
            Surface.ViewHeight = Surface.Height;
            foreach (var child in Children)
            {
                if (child is ControlsConsole console)
                {
                    console.IsVisible = true;
                }
            }
        }

        protected virtual void TextBox_OnEditModeEnter(object? o, EventArgs e)
        {
            Maximize();
        }

        protected virtual void TextBox_OnEditModeExit(object? o, EventArgs e)
        {
            Minimize();
        }
    }
}