using SadConsole.Components;

namespace SadExperiments.Pages;

/*
     * This class shows the difference in the number of calls to Update and Render in SadConsole. On some systems Render will be called less frequently than Update.
     * Or so we thought...
     * 
     * Chris3606 explains:
     * 
     * the number of times Render is called is system dependent, on my system I get something in the neighborhood of 49; 
     * the behavior is also host dependent, because for example if you switch to SMFL both Update and Render are called around 49 times on my system.  
     * Bottom line, the behavior is up to the back-end renderer and its config.
     * In MonoGame, that behavior appears to be a side effect of MonoGame's FixedTimeStep implementation: 
     * if you add the following line to your init, the number of times for each are 62 and 61, respectively, on my system, because then it's controlled exclusively by VSync:
     * Game.Instance.MonoGameInstance.IsFixedTimeStep = false;
     * As for exactly where/how SadConsole calls Render and why it leads to that behavior, I'd have to dig into SadConsole's MonoGame host.  
     * Interesting for sure, but practically I think it just indicates that you should use Update whenever you care about time step, which is generally good advice anyway.  
     * If I recall correctly it's not actually advisable to do any drawing to surfaces or anything in Render, that should be done in Update; 
     * Render is mostly just for rendering cached textures to the screen in the back-end. 
     * 
     * ----
     * 
     * (Rychu) When I inserted Chris' explanation above, I wanted to add the line Game.Instance.MonoGameInstance.IsFixedTimeStep = false; 
     * that Chris talks about, so the component (when finished the first pass) would reinsert an instance of itself to the host with the above modification, 
     * but by accident I forgot to paste it. To my astonishment, the second run of the component presented perfectly synced runs of both Update and Render.
     * 
     * ---
     * 
     * Chris3606 explains:
     * 
     * ... it makes sense that the first few would be laggy because it (presumably) involves actually initializing the renderer(s) 
     *
     */

class UpdateAndRender : Page
{
    public UpdateAndRender()
    {
        Title = "Update And Render";
        Summary = "Checking the difference between the Update and Render steps.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.IComponent };

        SadComponents.Add(new TestUpdateRenderComponent());
    }

    class TestUpdateRenderComponent : LogicComponent
    {
        static bool s_firstCall = true;
        int _offset = 10;
        int _updateCounter = 0;
        int _renderCounter = 0;
        TimeSpan _timeUpdate = TimeSpan.Zero;
        TimeSpan _timeRender = TimeSpan.Zero;

        public override void Update(IScreenObject so, TimeSpan delta)
        {
            Test(ref _updateCounter, ref _timeUpdate, delta, 1, (ScreenSurface)so);
        }

        public override void Render(IScreenObject so, TimeSpan delta)
        {
            Test(ref _renderCounter, ref _timeRender, delta, 3, (ScreenSurface)so);
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
                host.SadComponents.Add(new TestUpdateRenderComponent());
            }
        }

        void Test(ref int counter, ref TimeSpan time, TimeSpan delta, int y, ScreenSurface surface)
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