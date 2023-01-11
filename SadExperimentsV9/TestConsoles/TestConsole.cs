namespace SadExperimentsV9.TestConsoles;

internal class TestConsole : ScreenSurface
{
    public TestConsole() : base(Program.Width - 2, Program.Height - 2)
    {
        Surface.DefaultBackground = Color.LightBlue;
        Surface.DefaultForeground = Color.Black;
        Surface.Clear();
    }
}