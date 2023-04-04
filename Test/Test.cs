namespace TestProject;

class Test : Console
{
    public Test() : base(GameHost.Instance.ScreenCellsX, GameHost.Instance.ScreenCellsY)
    {
        Surface.Print(0, 0, " ");
        Surface.Print(0, 1, Surface.GetGlyph(0, 0).ToString());
    }
}