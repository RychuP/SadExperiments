using SadConsole;
using SadConsole.Readers;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadExperimentsV9
{
    public static class Extensions
    {
        /// <summary>
        /// Prints text centered on the surface.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="y"></param>
        /// <param name="c"></param>
        public static void Print(this ScreenSurface s, string t, int y, Color? c = null) =>
            s.Surface.Print(0, y, t.Align(HorizontalAlignment.Center, s.Surface.Width), c ?? s.Surface.DefaultForeground);
    }
}
