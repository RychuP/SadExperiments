using SadExperiments;

namespace SadExperiments.Pages.Sad_Console;

internal class FontChanging : Page
{
    public FontChanging()
    {
        Title = "Font Changing";
        Summary = "Importing font from a file and using it in a console.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.SadFont, Tag.Cursor, Tag.ScreenSurface };

        // load and change font
        Font = Fonts.Square10;
        FontSize = Font.GetFontSize(IFont.Sizes.One) * 1.5;

        // create an additional console with a default font
        var c = new Console(32, 7);
        c.DefaultBackground = Color.AnsiCyan;
        c.Clear();

        // print some info on the new console
        c.Print(1, 1, "Number of fonts: " + Game.Instance.Fonts.Count.ToString());
        c.Cursor.NewLine().Down(1).SetPrintAppearance(Color.Cyan, c.DefaultBackground)
            .PrintAppearanceMatchesHost = false;
        foreach (string key in Game.Instance.Fonts.Keys)
        {
            c.Cursor.NewLine().Right(4).Print(key);
        }

        // add the new console to the children
        Children.Add(c);
        c.Position = new Point(Surface.Width / 2 - c.Width / 2, 2);

        Surface.Print(7, 12, "Glyphs from a font Square10");

        // print some glyphs from the new font
        int x = 6, y = 14, glyphNumber = 48;
        for (int b = 1; b <= 6; b++)
        {
            for (int a = 0; a < 16; a++)
            {
                Surface.SetGlyph(x + a * 2, y + b * 2, glyphNumber++, Program.RandomColor);
            }
        }
    }
}