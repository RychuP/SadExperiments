using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using SadConsole.Components;
using SadExperimentsV9.Components;

namespace SadExperimentsV9.Consoles
{
    class UpdateAndRenderDifference : ScreenSurface
    {
        LogicComponent _testComp;
        
        public UpdateAndRenderDifference(int w, int h) : base(w, h)
        {
            _testComp = new TestComponent();
            SadComponents.Add(_testComp);
        }
    }
}
