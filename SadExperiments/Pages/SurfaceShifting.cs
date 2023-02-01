namespace SadExperiments.Pages;

class SurfaceShifting : Page
{
    public SurfaceShifting()
    {
        Title = "Surface Shifting";
        Summary = "Arrow keys ShiftDown, ShiftUp, ShiftLeft and ShiftRight surface.";
        DefaultBackground = Color.DarkBlue;
        AddCentered(new SurfaceShiftingExample());
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed)
        {
            if (keyboard.IsKeyPressed(Keys.Right))
                SubPage.Surface.ShiftLeft();
            else if (keyboard.IsKeyPressed(Keys.Left))
                SubPage.Surface.ShiftRight();
            else if (keyboard.IsKeyPressed(Keys.Up))
                SubPage.Surface.ShiftDown();
            else if (keyboard.IsKeyPressed(Keys.Down))
                SubPage.Surface.ShiftUp();
        }

        return base.ProcessKeyboard(keyboard);
    }
}

internal class SurfaceShiftingExample : SubPage
{
    public SurfaceShiftingExample()
    {
        Surface.Fill(glyph: '#');
    }
}