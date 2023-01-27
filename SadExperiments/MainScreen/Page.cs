using SadConsole.Quick;
using SadConsole.UI.Windows;

namespace SadExperiments.MainScreen;

class Page : Console
{
    static readonly ColorPickerPopup s_colorPicker = new();
    static readonly CharacterViewer s_characterViewer = new(1);

    static Page()
    {
        s_colorPicker.FontSize *= 0.9;
        s_colorPicker.Center();
        s_colorPicker.SelectedColor = Color.White;
        s_colorPicker.WithKeyboard((o, k) =>
        {
            if (k.HasKeysPressed && k.IsKeyPressed(Keys.F4))
            {
                s_colorPicker.Hide();
                return true;
            }
            return false;
        });
        s_characterViewer.Center();
    }

    public string Title { get; init; } = string.Empty;

    public string Summary { get; init; } = string.Empty;

    public int Index { get; set; }

    public SubPage SubPage
    {
        get => Children[0] as SubPage ?? throw new Exception("There is no SubPage added to the Children of this Page.");
        set => Children[0] = value;
    }

    public Page() : this(Program.Width, Program.Height) { }

    public Page(int w, int h) : base(Program.Width, Program.Height, w, h)
    {
        UsePixelPositioning = true;
        Position = (0, Header.Height * GameHost.Instance.DefaultFont.GlyphHeight);
    }

    public void AddCentered(ScreenSurface child)
    {
        Children.Add(child);
        child.UsePixelPositioning = true;

        int childWidth = child.WidthPixels;
        int x = WidthPixels / 2 - childWidth / 2;

        int childHeight = child.HeightPixels;
        int y = HeightPixels / 2 - childHeight / 2;
        y = y < 0 ? 0 : y;

        child.Position = (x, y);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed && Parent is Container container)
        {
            if (keyboard.IsKeyPressed(Keys.F1))
            {
                container.PrevPage();
                return true;
            }
            else if (keyboard.IsKeyPressed(Keys.F2))
            {
                container.NextPage();
                return true;
            }
            else if (keyboard.IsKeyPressed(Keys.F3))
            {
                container.ShowContentsList();
                return true;
            }
            else if (keyboard.IsKeyPressed(Keys.F4))
            {
                s_colorPicker.Show(true);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.F5))
            {
                s_characterViewer.Show(true);
                return true;
            }
        }

        return base.ProcessKeyboard(keyboard);
    }
}

internal interface IRestartable
{
    void Restart();
}