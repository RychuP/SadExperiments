using SadConsole.Components;
using SadConsole;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadExperimentsV9
{
    class TestComponent : LogicComponent
    {
        static bool s_firstCall = true;
        int _offset = 10;
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
            int y = s_firstCall ? 0 : _offset;

            if (host is ScreenSurface s)
            {
                s.Surface.Print(1, 5 + y, $"Update Counter: {_updateCounter}");
                s.Surface.Print(1, 6 + y, $"Render Counter: {_renderCounter}");
                s.Surface.Print(1, 7 + y, $"Update Time: {_timeUpdate}");
                s.Surface.Print(1, 8 + y, $"Render Time: {_timeRender}");
            }

            if (s_firstCall)
            {
                s_firstCall = !s_firstCall;
                //Game.Instance.MonoGameInstance.IsFixedTimeStep = false;
                host.SadComponents.Add(new TestComponent());
            }
        }

        void test(ref int counter, ref TimeSpan time, TimeSpan delta, int y, ScreenSurface surface)
        {
            y = s_firstCall ? y : y + _offset;

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
