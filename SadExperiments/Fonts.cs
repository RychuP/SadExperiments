using SadConsole.Readers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadExperiments
{
    static class Fonts
    {
        static readonly string FontsDirectoryPath = "./Resources/Fonts/";
        static HashSet<TheDrawFont> s_drawFonts = new();

        public static IFont Default => GameHost.Instance.DefaultFont;
        public static IFont ThickSquare8 => GetFont("ThickSquare8", "thick_square_8x8");
        public static IFont Square10 => GetFont("Square10", "square10");
        public static IFont Empty => GetFont("empty_font", "empty_font");
        public static TheDrawFont Destruct => GetDrawFont("Destruct", "DESTRUCX");


        static IFont GetFont(string fontName, string fontFileName)
        {
            if (GameHost.Instance.Fonts.ContainsKey(fontName))
                return GameHost.Instance.Fonts[fontName];
            else
            {
                try
                {
                    var font = GameHost.Instance.LoadFont(FontsDirectoryPath + fontFileName + ".font");
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
                var fontEnumerable = TheDrawFont.ReadFonts(FontsDirectoryPath + fontFileName + ".TDF");
                if (fontEnumerable is IEnumerable && 
                    fontEnumerable.Where(f => f.Title == fontName).FirstOrDefault() is TheDrawFont drawFont)
                {
                    s_drawFonts = s_drawFonts.Concat(fontEnumerable).ToHashSet();
                    return drawFont;
                } 
                else
                {
                    throw new ArgumentException($"There has been a problem while loading the DrawFont {fontName}.");
                }
            }
        }
    }
}
