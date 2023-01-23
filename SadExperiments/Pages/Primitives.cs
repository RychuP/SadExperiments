using GoRogue;
using GoRogue.Random;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadExperiments.MainScreen;
using ShaiRandom.Generators;
using System.Diagnostics.Metrics;

namespace SadExperiments.Pages;

internal class Primitives : Page
{
    readonly Player _player;
    readonly PlayerInfo _playerInfo;
    readonly ControlsConsole _buttons;
    readonly DeltaTime _deltaTime = new(0.25d);
    readonly Grid _grid;
    bool _isPaused = false;

    public Primitives()
    {
        Title = "Primitives";
        Summary = "Playing with SadRogue Primitive Point, Distance and similar (WIP).";

        Cursor.UseStringParser = true;

        _deltaTime.TresholdReached += DeltaTime_OnTresholdReached;

        // create the map
        var map = new Map()
        {
            Parent = this,
            Position = (1, 1)
        };
        _grid = map.Grid;

        // create a player
        _player = new(_grid.Area.Center, Math.Min(_grid.Area.Width, _grid.Area.Height));
        _player.PositionChanged += map.Lines.Player_OnPositionChanged;

        // create player info console
        int x = (map.WidthPixels + map.FontSize.X * 2) / FontSize.X;
        int width = Width - x;
        _playerInfo = new PlayerInfo(width, Height - 2)
        {
            Parent = this,
            Position = (x, 1)
        };
        _player.PositionChanged += Player_OnPositionChanged;

        // move cursor
        int mapHeight = (map.HeightPixels + map.FontSize.Y * 2) / FontSize.Y;
        _buttons = new Buttons(Width - _playerInfo.Width - 2, Height - mapHeight)
        {
            Parent = this,
            Position = (1, mapHeight)
        };
    }

    void Player_OnPositionChanged(object? sender, PositionChangedEventArgs args)
    {
        _playerInfo.PrintInfo(_player, args.LineLength);
    }

    void DeltaTime_OnTresholdReached(object? sender, EventArgs args)
    {
        Direction currentDirection = _player.Direction;

        if (_player.WantsToTurn())
        {
            // try turning the player left or right
            if (!TurnPlayer())
            {
                // try going straight ahead
                _player.Direction = currentDirection;
                if (_grid.IsWalkable(_player.NextPosition)) MovePlayer();
                else
                {
                    // dead end... turn the player back
                    _player.Direction = currentDirection + 4;
                }
            }
        }

        // player wants to go straight
        else
        {
            // try going straight ahead
            if (_grid.IsWalkable(_player.NextPosition)) MovePlayer();

            // try turning the player left or right
            else if (!TurnPlayer())
            {
                // dead end... turn the player back
                _player.Direction = currentDirection + 4;
            }
        }
    }

    bool TurnPlayer()
    {
        // turn the player left or right
        _player.Turn();
        if (_grid.IsWalkable(_player.NextPosition))
        {
            MovePlayer();
            return true;
        }
        else
        {
            // turn the player the other way
            _player.InverseTurn();
            if (_grid.IsWalkable(_player.NextPosition))
            {
                MovePlayer();
                return true;
            }
            return false;
        }
    }

    void MovePlayer()
    {
        _grid.SetCellToFloor(_player.Position);
        _player.Walk();
        _grid.SetCellAppearance(_player.Position.X, _player.Position.Y, _player);
    }

    public override void Update(TimeSpan delta)
    {
        if (!_isPaused) 
            _deltaTime.Add(delta);
        base.Update(delta);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed && keyboard.IsKeyPressed(Keys.Space))
            _isPaused = !_isPaused;
        return base.ProcessKeyboard(keyboard);
    }

    void Print(params string[] text)
    {
        foreach (string line in text)
            Cursor.NewLine().Print(line);
    }

    class Buttons : ControlsConsole
    {
        public Button ChangeLineAlgorithm { get; init; }

        public Button RedrawMap { get; init; }

        public Button ChangeLineVisibility { get; init; }

        public Buttons(int w, int h) : base(w, h)
        {
            int padding = 4;
            RedrawMap = CreateButton("Redraw Map", padding, 1);
            ChangeLineAlgorithm = CreateButton("Orthogonal Line", padding, 3);
            ChangeLineVisibility = CreateButton("Hide Line", padding, 5);
        }

        Button CreateButton(string label, int padding, int y)
        {
            var button = new Button(label.Length + padding, 1)
            {
                Text = label,
                UseMouse = true,
                UseKeyboard = false,
            };
            button.Position = (Width / 2 - button.Width / 2, y);
            Controls.Add(button);
            return button;
        }
    }

    class PlayerInfo : Console
    {
        ManhattanDistance _manhattanDistance = Distance.Manhattan;
        ChebyshevDistance _chebyshevDistance = Distance.Chebyshev;
        EuclideanDistance _euclideanDistance = Distance.Euclidean;

        public PlayerInfo(int w, int h) : base (w, h)
        {
            Cursor.UseStringParser = true;
        }

        public void PrintInfo(Player player, int lineLength)
        {
            Surface.Clear();
            Surface.DrawOutline();
            Cursor.Move((0, 2));

            // player internal data
            Print("Position:", player.Position);
            Print("Direction:", player.Direction);
            Print("Desired distance:", player.DesiredDistance);
            Print("Current distance:", player.CurrentDistance);

            // reference point
            Cursor.NewLine();
            double distToOrigin = Point.EuclideanDistanceMagnitude(player.Position);
            Point referencePoint = Point.Zero;
            Print("Reference point:", referencePoint);
            Print("BearingOfLine:", Point.BearingOfLine(player.Position).ToString("f2"));
            Print("Line length:", lineLength);
            
            // distances
            Cursor.NewLine();
            Print("EuclideanDistanceMagnitude:", distToOrigin.ToString("f2"));
            Print("Manhattan distance", _manhattanDistance.Calculate(referencePoint, player.Position));
            Print("Chebyshev distance", _chebyshevDistance.Calculate(referencePoint, player.Position));
            Print("Euclidean distance", $"{_euclideanDistance.Calculate(referencePoint, player.Position):f2}");

            
        }

        void Print(string label, object value)
        {
            Cursor.Right(2).Print($"[c:r f:LightBlue]{label,-30}[c:undo] {value.ToString()}").NewLine();
        }
    }

    class PositionChangedEventArgs : EventArgs
    {
        public int LineLength { get; set; } = 0;

        public Point Position { get; set; } = Point.Zero;

        public PositionChangedEventArgs(Point position) => Position = position;
    }

    class Map : ScreenSurface
    {
        public Grid Grid { get; init; }

        public LineLayer Lines { get; init; }

        public Map() : base(11, 11)
        {
            // change font
            Font = Fonts.Square10;
            FontSize *= Grid.FontSizeMultiplier;

            // row numbers
            for (int x = 0, count = Surface.Width - 1; x < count; x++)
                Surface.SetGlyph(x + 1, 0, '0' + (x % 10), Color.LightGreen);

            // column numbers
            for (int y = 0, count = Surface.Height - 1; y < count; y++)
                Surface.SetGlyph(0, y + 1, '0' + (y % 10), Color.LightGreen);

            int width = Surface.Width - 1;
            int height = Surface.Height - 1;

            Grid = new Grid(width, height)
            {
                Parent = this,
                Position = (1, 1)
            };

            Lines = new LineLayer(width, height)
            {
                Parent = this,
                FontSize = this.FontSize,
                Position = (1, 1)
            };
        }

        public void Redraw() { }
    }

    class LineLayer : ScreenSurface
    {
        ColoredGlyph appearance = new(Color.LightSkyBlue, Color.Transparent, 'X');

        public LineLayer(int w, int h) : base(w, h) { }

        int DrawLine(Point start, Point end, Lines.Algorithm algorithm)
        {
            Surface.Clear();
            var linePoints = Lines.Get(start, end, algorithm);
            foreach (var point in linePoints)
                if (point != start)
                    Surface.SetGlyph(point.X, point.Y, appearance);
            return linePoints.Count();
        }

        public void Player_OnPositionChanged(object? sender, PositionChangedEventArgs args)
        {
            args.LineLength = DrawLine(args.Position, Point.Zero, Lines.Algorithm.Bresenham);
        }
    }

    class Grid : Console
    {
        public const char FloorGlyph = '.';
        public const char WallGlyph = '#';
        public const int FontSizeMultiplier = 2;

        public Grid(int w, int h) : base(w, h)
        {
            // change font
            Font = Fonts.Square10;
            FontSize *= FontSizeMultiplier;

            // fill grid with floors
            Surface.Fill(glyph: FloorGlyph);

            // draw some random walls
            var rand = GlobalRandom.DefaultRNG;
            int wallCount = rand.NextInt(2, 6);
            for (int i = 0; i < wallCount; i++)
            {
                Point start = rand.RandomPosition(Surface.Area);
                Direction direction = rand.RandomCardinalDirection();
                int length = rand.NextInt(2, 6);
                int deltaX = direction.DeltaX * length;
                int deltaY = direction.DeltaY * length;
                Point end = start + (deltaX, deltaY);
                Surface.DrawLine(start, end, WallGlyph, Color.LightCoral);
            }
        }

        public void SetCellToFloor(Point position) =>
            Surface.SetGlyph(position.X, position.Y, FloorGlyph, Surface.DefaultForeground);

        public void MoveGlyph(Point oldPos, Point newPos)
        {
            if (!Surface.Area.Contains(oldPos) || !Surface.Area.Contains(newPos) || oldPos == newPos) return;
            var appearance = Surface.GetCellAppearance(oldPos.X, oldPos.Y);
            Surface.SetCellAppearance(newPos.X, newPos.Y, appearance);
            SetCellToFloor(oldPos);
        }

        public bool IsWalkable(Point position) =>
            Surface.Area.Contains(position) &&
            Surface.GetGlyph(position.X, position.Y) == FloorGlyph;
    }

    class Player : ColoredGlyph
    {
        int _maxDistance;
        int _desiredDistance;
        int _currentDistance;
        Point _position;
        
        public Direction Direction { get; set; }
        public int DesiredDistance => _desiredDistance;
        public int CurrentDistance => _currentDistance;

        public Player(Point position, int maxDistance) : base(Color.Yellow, Color.Black, '@')
        {
            Position = position;
            _maxDistance = maxDistance;
            Direction = GlobalRandom.DefaultRNG.RandomCardinalDirection();
            SetDesiredDistance();
        }

        public Point Position
        {
            get => _position;
            set
            {
                _position = value;
                var args = new PositionChangedEventArgs(_position);
                OnPositionChange(args);
            }
        }

        protected virtual void OnPositionChange(PositionChangedEventArgs args) =>
            PositionChanged?.Invoke(this, args);

        public event EventHandler<PositionChangedEventArgs>? PositionChanged;

        public void Turn()
        {
            Direction += GlobalRandom.DefaultRNG.NextBool() ? 2 : -2;
            SetDesiredDistance();
        }

        public void InverseTurn() => Direction += 4;

        public bool WantsToTurn() => _currentDistance == _desiredDistance;

        public void Walk()
        {
            Position = NextPosition;
            _currentDistance++;
        }

        public Point NextPosition => Position + Direction;

        void SetDesiredDistance()
        {
            _desiredDistance = GlobalRandom.DefaultRNG.NextInt(2, _maxDistance);
            _currentDistance = 0;
        }
    }
}   