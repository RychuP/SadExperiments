using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadConsole;
using SadRogue.Primitives;

namespace SadExperimentsV9
{
    class TestConsole : ScreenSurface
    {
        public TestConsole() : base(Program.Width, Program.Height) { }

        protected void AddCentered(ScreenSurface a)
        {
            float fontSizeRatioX = (float)Font.GetFontSize(IFont.Sizes.One).X / a.Font.GetFontSize(IFont.Sizes.One).X,
                fontSizeRatioY = (float)Font.GetFontSize(IFont.Sizes.One).Y / a.Font.GetFontSize(IFont.Sizes.One).Y;
            int x = Convert.ToInt32((Surface.Width * fontSizeRatioX - a.Surface.Width) / 2),
                y = Convert.ToInt32((Surface.Height * fontSizeRatioY - a.Surface.Height) / 2);
            a.Position = (x, y);
            Children.Add(a);
        }
    }
}
