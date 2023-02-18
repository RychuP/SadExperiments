using SadConsole.Entities;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadExperiments.UI;

namespace SadExperiments.Pages;

internal class Test : Page
{
    Entity _square;

    public Test()
    {
        Font = Fonts.Square10;
        FontSize *= 2;

        var renderer = new Renderer();
        SadComponents.Add(renderer);

        var rectangle = new Rectangle(1, 1, 12, 22);
        Surface.DrawRectangle(rectangle, Color.Pink);

        var entity = new Entity(Color.Cyan, Color.Transparent, Font.SolidGlyphIndex, 1);
        entity.Position = (rectangle.MinExtentX + 1, rectangle.MaxExtentY - 1);
        renderer.Add(entity);

        _square = new Entity(Color.Red, Color.Transparent, Font.SolidGlyphIndex, 1);
        _square.Position = (rectangle.MinExtentX + 3, rectangle.MaxExtentY - 1);
        renderer.Add(_square);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed)
        {
            int x = _square.Position.X;

            if (keyboard.IsKeyPressed(Keys.Left))
            {
                _square.Position = _square.Position.WithX(x - 1);
            }
            else if (keyboard.IsKeyPressed(Keys.Right))
            {
                _square.Position = _square.Position.WithX(x + 1);
            }
        }

        return base.ProcessKeyboard(keyboard);
    }
}