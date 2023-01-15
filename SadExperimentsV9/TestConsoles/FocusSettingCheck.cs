using SadConsole.Components;
using Console = SadConsole.Console;

namespace SadExperimentsV9.TestConsoles
{
    internal class FocusSettingCheck : TestConsole
    {
        public FocusSettingCheck()
        {
            _ = new ClickableConsole()
            {
                Parent = this,
                Position = (1, 1),
                IsFocused = true
            };

            _ = new ClickableConsole()
            {
                Parent = this,
                Position = (35, 1),
                IsFocused = true
            };

            Surface.Print(1, 5, "Click on the above consoles and notice how their focus automatically changes.");
        }

        class ClickableConsole : Console
        {
            public ClickableConsole() : base(20, 3)
            {
                DefaultBackground = Color.Black;
                DefaultForeground = Color.White;
                FocusOnMouseClick = true;

                // just playing with components
                SadComponents.Add(new ShowFocus());
            }
        }
        
        class ShowFocus : UpdateComponent
        {
            public override void Update(IScreenObject host, TimeSpan delta)
            {
                if (host is Console c)
                {
                    c.Clear();
                    c.Print(1, 1, $"IsFocused: {c.IsFocused}");
                }
            }
        }
    }
}
