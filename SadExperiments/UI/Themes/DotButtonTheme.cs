using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadExperiments.UI.Controls;

namespace SadExperiments.UI.Themes;

class DotButtonTheme : ThemeBase
{
    public DotButtonTheme(int normalGlyph = 9, int selectedGlyph = 4)
    {
        ControlThemeState.SetGlyph(normalGlyph);
        ControlThemeState.Selected.Glyph = selectedGlyph;
        ControlThemeState.SetForeground(Color.LightBlue);
        ControlThemeState.Selected.Foreground = Color.LightGreen;
        ControlThemeState.MouseOver.Foreground = Color.Yellow;
    }

    public override void UpdateAndDraw(ControlBase control, TimeSpan time)
    {
        if (control is not DotButton dotButton) return;
        if (!dotButton.IsDirty) return;

        //RefreshTheme(dotButton.FindThemeColors(), dotButton);    <- this messes up the intended appearance
        ColoredGlyph appearance = ControlThemeState.GetStateAppearance(dotButton.State);

        appearance.CopyAppearanceTo(dotButton.Surface[0]);

        // reset the glyph because of the mouseover state which will set its glyph regardles of the selected state
        dotButton.Surface[0].Glyph = dotButton.IsSelected ? 
            ControlThemeState.Selected.Glyph : ControlThemeState.Normal.Glyph;

        dotButton.IsDirty = false;
    }

    public override ThemeBase Clone() 
    {
        return new DotButtonTheme()
        {
            ControlThemeState = ControlThemeState.Clone(),
        };
    }
}