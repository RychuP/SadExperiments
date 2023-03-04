using System.IO;
using SadConsole.Readers;
using System.Collections;

namespace SadExperiments;

static class Fonts
{
    static readonly string FontsDirectoryPath = Path.Combine("Resources", "Fonts");
    static HashSet<TheDrawFont> s_drawFonts = new();

    public static IFont Default => GameHost.Instance.DefaultFont;
    public static IFont ThickSquare8 => GetFont("ThickSquare8", "thick_square_8x8");     // background not transparent
    public static IFont Square10 => GetFont("square10");                                 // background not transparent
    public static IFont C64 => GetFont("C64");
    public static IFont C64_petscii => GetFont("c64_petscii");
    public static IFont Cheepicus12 => GetFont("Cheepicus12");
    public static IFont Jpetscii => GetFont("jpetscii");
    public static IFont Empty => GetFont("empty_font");                                 // for pixel manipulations
    public static IFont Maze => GetFont("maze");                                        // pacman maze
    public static IFont Sprites => GetFont("sprites");                                  // pacman sprites
    public static TheDrawFont Destruct => GetDrawFont("Destruct", "DESTRUCX");


    static IFont GetFont(string fontName, string fontFileName = "")
    {
        if (GameHost.Instance.Fonts.ContainsKey(fontName))
            return GameHost.Instance.Fonts[fontName];
        else
        {
            try
            {
                fontFileName = fontFileName == string.Empty ? fontName : fontFileName;
                string path = Path.Combine(FontsDirectoryPath, fontFileName + ".font");
                var font = GameHost.Instance.LoadFont(path);
                return font;
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                throw new ArgumentException($"There has been a problem while loading the font {fontName}.");
            }
        }
    }

    static TheDrawFont GetDrawFont(string fontName, string fontFileName)
    {
        if (s_drawFonts.Where(f => f.Title == fontName) is TheDrawFont df)
            return df;
        else
        {
            string path = Path.Combine(FontsDirectoryPath, fontFileName + ".TDF");
            var fontEnumerable = TheDrawFont.ReadFonts(path);
            if (fontEnumerable is IEnumerable &&
                fontEnumerable.Where(f => f.Title == fontName).FirstOrDefault() is TheDrawFont drawFont)
            {
                s_drawFonts = s_drawFonts.Concat(fontEnumerable).ToHashSet();
                return drawFont;
            }
            else
                throw new ArgumentException($"There has been a problem while loading the DrawFont {fontName}.");
        }
    }
}