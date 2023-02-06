using SadConsole.UI.Controls;
using SadConsole.UI.Themes;

namespace SadExperiments.MainScreen;

class ClearButton : Button
{
    const string ShortTitle = "X";
    const string LongTitle = "Clear";
    const int Padding = 2;
    int _x = 0;

    static ClearButton() =>
        Library.Default.SetControlTheme(typeof(ClearButton), new ButtonTheme());

    public ClearButton() : base(ShortTitle.Length + Padding, 1)
    {
        Text = ShortTitle;
        CanFocus = false;
    }

    protected override void OnMouseEnter(ControlMouseState state)
    {
        _x = Position.X;
        Text = LongTitle;
        Resize(LongTitle.Length + Padding, 1);
        Position = Position.WithX(_x - LongTitle.Length + 1);
        base.OnMouseEnter(state);
    }

    protected override void OnMouseExit(ControlMouseState state)
    {
        Text = ShortTitle;
        Resize(ShortTitle.Length + Padding, 1);
        Position = Position.WithX(_x);
        base.OnMouseExit(state);
    }
}
