using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadConsole;
using SadRogue.Primitives;

namespace SadExperimentsV9.TestConsoles
{
    class TestConsole : ScreenSurface
    {
        public TestConsole() : base(Program.Width / 2, Program.Height / 2)
        {
            Surface.DefaultBackground = Color.LightBlue;
            Surface.DefaultForeground = Color.Black;
            Surface.Clear();
            FontSize = (8, 16);
            Surface.Print(1, 1, FontSize.ToString());
        }
    }
}
