using SadConsole.UI;
using SadConsole.UI.Themes;
using SadExperiments.UI.Controls;
using SadExperiments.UI.Themes;

namespace SadExperiments.UI;

class DotNavigation : ControlsConsole
{
    int _maximum;
    int _value;

    public DotNavigation(int maximum = 1) : base(2 * maximum - 1, 1)
    {
        if (maximum <= 0)
            throw new ArgumentException("Dot count cannot be zero or negative.");

        // index 7 in the C64 font for the selected glyph
        var dotTheme = new DotButtonTheme(selectedGlyph: 7);
        Library.Default.SetControlTheme(typeof(DotButton), dotTheme);

        UsePixelPositioning = true;
        Font = Fonts.C64;
        Maximum = maximum;
    }

    /// <summary>
    /// Distance between dots.
    /// </summary>
    public int Padding { get; set; } = 1;

    /// <summary>
    /// Index of the currently selected dot.
    /// </summary>
    public int Value
    {
        get => _value;
        set
        {
            if (value < 0 || value >= Maximum) return;
            _value = value;
            if (Controls[_value] is DotButton dotButton)
                dotButton.IsSelected = true;
            OnSelectedDotChanged();
        }
    }

    /// <summary>
    /// Number of dots in the navigation.
    /// </summary>
    public int Maximum
    {
        get => _maximum;
        set
        {
            if (value != _maximum)
            {
                // clamp value
                _maximum = value >= 1 ? value : 1;

                // remove previous controls
                Controls.Clear();

                // resize to accommodate new count of dots
                int width = Maximum + Padding * (Maximum - 1);
                Resize(width, 1, width, 1, true);

                // create and add new dots
                for (int i = 0, x = 0; i < _maximum; i++, x += Padding + 1)
                {
                    var dotButton = new DotButton() { Position = (x, 0) };
                    dotButton.Click += (o, e) => 
                        Value = Controls.IndexOf(dotButton);
                    Controls.Add(dotButton);
                }

                // select the initial dot
                Value = 0;
            }
        }
    }

    protected virtual void OnSelectedDotChanged() =>
        SelectedDotChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? SelectedDotChanged;
}