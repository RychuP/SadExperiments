using System;
using SadConsole;
using SadConsole.Entities;
using SadConsole.Input;
using SadRogue.Primitives;
using SadExperimentsV9.Entities;
using Console = SadConsole.Console;

namespace SadExperimentsV9.TestConsoles
{
    class CheckeredRoom : Console
    {
        Player player;
        Renderer entityManager;

        public CheckeredRoom() : base(Program.Width, Program.Height, Program.Width * 2, Program.Height * 2)
        {
            // setup surface
            DefaultBackground = Color.DarkGray;
            this.Clear();
            FillWithCheckers();
            IsFocused = true;

            // border
            ColoredGlyph glyph = new(Color.Brown, Color.Black, 177);
            Rectangle rectangle = new(0, 0, Width, Height);
            this.DrawBox(rectangle, ShapeParameters.CreateBorder(glyph));

            // create player
            player = new Player();
            SadComponents.Add(new SadConsole.Components.SurfaceComponentFollowTarget() { Target = player });

            // entity manager
            entityManager = new Renderer();
            SadComponents.Add(entityManager);
            entityManager.Add(player);
        }

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            if (keyboard.HasKeysPressed)
            {
                Point direction = (0, 0);

                if (keyboard.IsKeyDown(Keys.Up))
                {
                    direction += Direction.Up;
                }
                else if (keyboard.IsKeyDown(Keys.Down))
                {
                    direction += Direction.Down;
                }

                if (keyboard.IsKeyDown(Keys.Left))
                {
                    direction += Direction.Left;
                }
                else if (keyboard.IsKeyDown(Keys.Right))
                {
                    direction += Direction.Right;
                }

                // check if the direction has changed at all
                if (direction.X != 0 || direction.Y != 0)
                {
                    Point point = player.GetNextMove(direction);
                    if (IsWalkable(point))
                    {
                        player.MoveTo(point);
                    }
                }
            }
            return base.ProcessKeyboard(keyboard);
        }

        bool IsWalkable(Point p)
        {
            if (p.Y <= 0 || p.Y == Height - 1 ||
                p.X <= 0 || p.X == Width - 1)
            { return false; }
            else
            { return true; }
        }

        void FillWithCheckers()
        {
            Color color = Color.DimGray;
            for (int y = 0; y < Height; y++)
            {
                if (y % 2 == 0)
                {
                    for (int x = 0; x < Width; x += 3)
                    {
                        this.Print(x, y, new ColoredGlyph(Program.RandomColor, color, 48));
                    }
                }
            }
        }
    }
}