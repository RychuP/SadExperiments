using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadExperiments;

/// <summary>
/// Displays the glyphs associated with a font and allows the user to select one.
/// Taken from Thraka's Feature Demo repo.
/// </summary>
public class CharacterViewer : Window
{
    private CharacterPicker _picker;
    private Label _label;
    private int _selectedCharacter;

    public Mirror MirrorEffect { get { return _picker.MirrorSetting; } set { _picker.MirrorSetting = value; } }

    public int SelectedCharacter
    {
        get => _selectedCharacter;
        set
        {
            _picker.SelectedCharacterChanged -= SelectedCharacterChangedOnControl;
            _selectedCharacter = _picker.SelectedCharacter = value;
            _picker.SelectedCharacterChanged += SelectedCharacterChangedOnControl;
        }
    }

    public CharacterViewer(int character)
        : base(20, 20)
    {
        FontSize *= 1.4;

        _picker = new CharacterPicker(SadConsole.UI.Themes.Library.Default.Colors.OrangeDark, SadConsole.UI.Themes.Library.Default.Colors.ControlBackgroundNormal, SadConsole.UI.Themes.Library.Default.Colors.Yellow);
        _picker.Position = new Point(2, 1);
        _picker.SelectedCharacter = character;
        _picker.UseFullClick = false;
        _picker.SelectedCharacterChanged += SelectedCharacterChangedOnControl;
        Controls.Add(_picker);

        _label = new Label(Width - 6);
        _label.TextColor = Color.White;
        _label.Position = (1, _picker.Bounds.MaxExtentY + 2);
        Controls.Add(_label);

        var closeButton = new Button(7);
        closeButton.Text = "Close";
        closeButton.Position = (Width - 8, _label.Position.Y);
        closeButton.Click += (s, e) => this.Hide();
        Controls.Add(closeButton);

        this.CloseOnEscKey = true;
        this.Title = "Pick a character";
    }

    private void SelectedCharacterChangedOnControl(object sender, ValueChangedEventArgs<int> e)
    {
        SelectedCharacter = e.NewValue;
        _label.DisplayText = $"{SelectedCharacter} - {(char)SelectedCharacter}";
        //this.Hide();
    }

    protected override void DrawBorder()
    {
        base.DrawBorder();

        if (_picker == null) return;

        var themeColors = Controls.GetThemeColors();

        var fillStyle = new ColoredGlyph(themeColors.ControlHostForeground, themeColors.ControlHostBackground);
        var borderStyle = new ColoredGlyph(themeColors.Lines, fillStyle.Background, 0);

        Surface.DrawLine(new Point(0, _picker.Bounds.MaxExtentY + 1), new Point(Width, _picker.Bounds.MaxExtentY + 1), BorderLineStyle[0], borderStyle.Foreground);
        Surface.ConnectLines(BorderLineStyle);
    }

    public override bool ProcessKeyboard(Keyboard info)
    {
        if (info.HasKeysPressed && info.IsKeyPressed(Keys.F5))
        {
            Hide();
            return true;
        }
        return base.ProcessKeyboard(info);
    }
}
