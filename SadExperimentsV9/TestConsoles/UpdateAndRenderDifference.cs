using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using SadConsole.Components;
using SadExperimentsV9.Components;

namespace SadExperimentsV9.TestConsoles
{
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

    class UpdateAndRenderDifference : ScreenSurface
    {
        public UpdateAndRenderDifference(int w, int h) : base(w, h)
        {
            SadComponents.Add(new TestUpdateRenderComponent());
        }
    }
}
