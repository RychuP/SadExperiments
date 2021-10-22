using SadConsole.Components;
using SadConsole;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadExperimentsV9.Components
{
    class TestComponent : LogicComponent
    {
        int _updateCounter = 0;
        int _renderCounter = 0;
        TimeSpan _timeUpdate = TimeSpan.Zero;
        TimeSpan _timeRender = TimeSpan.Zero;

        public override void Update(IScreenObject so, TimeSpan delta)
        {
            test(ref _updateCounter, ref _timeUpdate, delta, 1, (ScreenSurface) so);
        }

        public override void Render(IScreenObject so, TimeSpan delta)
        {
            test(ref _renderCounter, ref _timeRender, delta, 3, (ScreenSurface)so);
        }

        public override void OnRemoved(IScreenObject host)
        {
            if (host is ScreenSurface s)
            {
                s.Surface.Print(1, 5, $"Update Counter: {_updateCounter}");
                s.Surface.Print(1, 6, $"Render Counter: {_updateCounter}");
                s.Surface.Print(1, 7, $"Update Time: {_timeUpdate}");
                s.Surface.Print(1, 8, $"Render Time: {_timeRender}");
            }
        }

        void test(ref int counter, ref TimeSpan time, TimeSpan delta, int y, ScreenSurface surface)
        {
            time += delta;
            if (time > TimeSpan.FromSeconds(1d))
            {
                surface.SadComponents.Remove(this);
            }
            else
            {
                int x = 1 + counter++;
                if (x > surface.Surface.Width)
                {
                    surface.SadComponents.Remove(this);
                }
                else
                {
                    surface.Surface.Print(x, y, ".");
                }
            }
        }
    }
}
