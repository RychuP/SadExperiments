using System;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace SadExperimentsV9
{
    class MapScreen : Console
    {
        public ColoredGlyph PlayerGlyph;
        ColoredGlyph _playerPositionMapGlyph;
        Point _playerPosition = new(4, 4);

        public MapScreen() : base(Program.Width, Program.Height)
        {
            Program.ReplaceSC(this);
            IsFocused = true;

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
                if (value.X < 0 || value.X >= Width ||
                    value.Y < 0 || value.Y >= Height)
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

            return false;
        }
    }
}
