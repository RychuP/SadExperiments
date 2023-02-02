namespace SadExperiments.Pages.Sad_Console;

class MovableCharacter : Page
{
    public ColoredGlyph PlayerGlyph;
    ColoredGlyph _playerPositionMapGlyph;
    Point _playerPosition = new(4, 4);

    public MovableCharacter()
    {
        Title = "Movable Character";
        Summary = "My early attempts at creating a player entity with keyboard input.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Primitives, Tag.Input, Tag.Keyboard, Tag.Drawing, Tag.Color };

        // draw central diagonal lines
        Point topLeft = (1, 1);
        Point topRight = (Width - 2, 1);
        Point bottomLeft = (1, Height - 2);
        Point bottomRight = (Width - 2, Height - 2);
        this.DrawLine(topLeft, bottomRight, 177, Color.LightGreen);
        this.DrawLine(bottomLeft, topRight, 178, Color.LightBlue);

        // draw offsetted diagonal lines
        Point offsetX = (8, 0);
        Point offsetY = (0, 3);
        this.DrawLine(topLeft + offsetX, bottomRight + offsetX, 179, Color.LightPink);
        this.DrawLine(topLeft + offsetY, bottomRight + offsetY, 179, Color.LightSlateGray);
        this.DrawLine(bottomLeft + offsetX, topRight + offsetX, 180, Color.LightYellow);
        this.DrawLine(bottomLeft - offsetY, topRight - offsetY, 181, Color.LightGreen);

        // border
        ColoredGlyph glyph = new(Color.Violet, Color.Black, 177);
        Rectangle rectangle = new(0, 0, Width, Height);
        this.DrawBox(rectangle, ShapeParameters.CreateBorder(glyph));

        // player
        PlayerGlyph = new ColoredGlyph(Color.White, Color.Black, 1);
        _playerPositionMapGlyph = new ColoredGlyph();
        _playerPositionMapGlyph.CopyAppearanceFrom(this[_playerPosition.X, _playerPosition.Y]);
        PlayerGlyph.CopyAppearanceTo(this[_playerPosition.X, _playerPosition.Y]);
    }

    public Point PlayerPosition
    {
        get => _playerPosition;
        private set
        {
            // Test new position
            if (value.X < 1 || value.X >= Width - 1 ||
                value.Y < 1 || value.Y >= Height - 1)
                return;

            // Restore map cell
            _playerPositionMapGlyph.CopyAppearanceTo(this[_playerPosition.X, _playerPosition.Y]);

            // Move player
            _playerPosition = value;

            // Save map cell
            _playerPositionMapGlyph.CopyAppearanceFrom(this[_playerPosition.X, _playerPosition.Y]);

            // Draw player
            PlayerGlyph.CopyAppearanceTo(this[_playerPosition.X, _playerPosition.Y]);

            // Redraw the map
            IsDirty = true;
        }
    }

    public override bool ProcessKeyboard(Keyboard info)
    {
        Point newPlayerPosition = PlayerPosition;

        if (info.IsKeyPressed(Keys.Up))
            newPlayerPosition += Direction.Up;
        else if (info.IsKeyPressed(Keys.Down))
            newPlayerPosition += Direction.Down;

        if (info.IsKeyPressed(Keys.Left))
            newPlayerPosition += Direction.Left;
        else if (info.IsKeyPressed(Keys.Right))
            newPlayerPosition += Direction.Right;

        if (newPlayerPosition != PlayerPosition)
        {
            PlayerPosition = newPlayerPosition;
            return true;
        }

        return base.ProcessKeyboard(info);
    }
}