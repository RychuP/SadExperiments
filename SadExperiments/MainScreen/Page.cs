namespace SadExperiments.MainScreen;

// page with some bite size SadConsole related content
//
// make sure to add handling of this page ProcessKeyboard() to any screen object you add to it that may possibly steal focus
// 
// if you use HorizontalButtonsConsole or VerticalButtonsConsole for adding buttons this process is automated in a way
// (check out AreaPage or RectangleBisection for an example)
class Page : Console
{
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
                container.ToggleContentsList();
                return true;
            }
            else if (keyboard.IsKeyPressed(Keys.F4))
            {
                Container.ColorPicker.Show(true);
                return true;
            }
            else if (keyboard.IsKeyPressed(Keys.F5))
            {
                Container.CharacterViewer.Show(true);
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