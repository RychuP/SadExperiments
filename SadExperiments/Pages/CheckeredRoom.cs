using SadConsole.Entities;
using SadExperiments.Entities;
using SadConsole.Components;

namespace SadExperiments.Pages;

class CheckeredRoom : Page
{
    readonly Player _player;
    readonly Renderer _entityManager;

    public CheckeredRoom() : base(Program.Width * 2, Program.Height * 2)
    {
        Title = "Checkered Room";
        Summary = "Basic map testing kb input and a component centering surface on player.";

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
        _player = new Player();
        SadComponents.Add(new SurfaceComponentFollowTarget() { Target = _player });

        // entity manager
        _entityManager = new Renderer();
        SadComponents.Add(_entityManager);
        _entityManager.Add(_player);
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
                Point point = _player.GetNextMove(direction);
                if (IsWalkable(point))
                {
                    _player.MoveTo(point);
                    return true;
                }

                // diagonal movement exception: check if one of the directions is walkable
                else if (direction.X != 0 && direction.Y != 0)
                {
                    point = _player.GetNextMove(direction.WithY(0));
                    if (IsWalkable(point))
                    {
                        _player.MoveTo(point);
                        return true;
                    }
                    else
                    {
                        point = _player.GetNextMove(direction.WithX(0));
                        if (IsWalkable(point))
                        {
                            _player.MoveTo(point);
                            return true;
                        }
                    }
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
                    Surface.SetGlyph(x, y, new ColoredGlyph(Program.RandomColor, color, 48));
                }
            }
        }
    }
}