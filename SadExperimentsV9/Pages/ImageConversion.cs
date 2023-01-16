namespace SadExperiments.Pages;

internal class ImageConversion : Page
{
    public ImageConversion()
    {
        Title = "Image Conversion";
        Summary = "Converting image files and checking brightness and conversion glyphs.";

        // convert 4 pixel vertical image file
        var image = GameHost.Instance.GetTexture("Resources/Images/test_opacity.png");
        var s = image.ToSurface(TextureConvertMode.Foreground, 1, 4);
        s.DefaultBackground = Color.Black;
        if ((s as CellSurface)?.Cells is ColoredGlyph[] a)
        {
            Array.ForEach(a, (c) => { c.Background = Color.Black; });
        }
        var surface = new ScreenSurface(s) { Parent = this };
        surface.Position = (1, 2);
        PrintInfo(s as CellSurface, 2);

        // convert the second 4 pixel image file with different colors
        image = GameHost.Instance.GetTexture("Resources/Images/test_opacity2.png");
        var s2 = image.ToSurface(TextureConvertMode.Foreground, 1, 4);
        var surface2 = new ScreenSurface(s2) { Parent = this };
        surface2.Position = (1, 7);
        PrintInfo(s2 as CellSurface, 7);

        void PrintInfo(CellSurface? s, int y)
        {
            if (s is null) return;

            for (int i = 0; i < 4; i++)
            {
                this.Print(3, y + i, $"Glyph: {Align(s[i].Glyph)}, Brightness: {Align(s[i].Foreground.GetBrightness())}, FG: {s[i].Foreground}");
            }
        }

        string Align(object i) => i.ToString().Align(HorizontalAlignment.Left, 3);
    }
}