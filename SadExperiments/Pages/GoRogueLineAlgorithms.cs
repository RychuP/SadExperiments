using GoRogue;
using GoRogue.Random;
using SadConsole.Ansi;
using SadConsole.Quick;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadExperiments.MainScreen;
using ShaiRandom.Generators;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.Serialization;

namespace SadExperiments.Pages;

internal class GoRogueLineAlgorithms : Page
{
    readonly PlayerInfo _playerInfo;
    readonly Buttons _buttons;
    readonly Map _map;

    public GoRogueLineAlgorithms()
    {
        Title = "GoRogue Line Algorithms";
        Summary = "Experimenting with lines, Point, Distance and similar primitives.";

        Cursor.UseStringParser = true;

        // create the map
        _map = new Map()
        {
            Parent = this,
            Position = (1, 1)
        };

        // create player info console
        int x = (_map.WidthPixels + _map.FontSize.X * 2) / FontSize.X;
        int width = Width - x;
        _playerInfo = new PlayerInfo(width, Height - 2)
        {
            Parent = this,
            Position = (x, 1)
        };
        _map.Player.PositionChanged += Player_OnPositionChanged;

        // create buttons
        int mapHeight = (_map.HeightPixels + _map.FontSize.Y * 2) / FontSize.Y;
        _buttons = new Buttons(Width - _playerInfo.Width - 2, Height - mapHeight, _map.Lines.CurrentAlgorithm.ToString())
        {
            Parent = this,
            Position = (1, mapHeight)
        };
        _buttons.RedrawMap.Click += (o, e) => _map.Redraw();
        _buttons.WithKeyboard((so, k) => ProcessKeyboard(k));

        _buttons.ChangeLineAlgorithm.Click += (o, e) =>
        {
            _map.Lines.ChangeAlgorithm();
            if (o is CustomButton b) b.Text = $"2. {_map.Lines.CurrentAlgorithm}";
            if (!_map.Player.IsMoving)
            {
                int length = _map.Lines.DrawLine(_map.Player.Position, PlayerInfo.ReferencePoint);
                _playerInfo.PrintInfo(_map.Player, length);
            }
        };

        _buttons.ShowHideLine.Click += (o, e) =>
        {
            _map.Lines.IsVisible = !_map.Lines.IsVisible;
            if (o is CustomButton b) b.Text = _map.Lines.IsVisible ? "3. Hide Line" : "3. Show Line";
        };

        _buttons.StartStopMovement.Click += (o, e) =>
        {
            _map.Player.IsMoving = !_map.Player.IsMoving;
            if (o is CustomButton b) b.Text = _map.Player.IsMoving ? "4. Stop Movement" : "4. Start Movement";
        };
    }

    public override bool ProcessKeyboard(SadConsole.Input.Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed)
        {
            foreach (var control in _buttons.Controls)
            {
                if (control is CustomButton cb && keyboard.IsKeyPressed(cb.KeyboardShortcut))
                {
                    cb.SimulateClick();
                    return true;
                }
            }
        }
        return base.ProcessKeyboard(keyboard);
    }

    void Player_OnPositionChanged(object? sender, PositionChangedEventArgs args)
    {
        _playerInfo.PrintInfo(_map.Player, args.LineLength);
    }

    void Print(params string[] text)
    {
        foreach (string line in text)
            Cursor.NewLine().Print(line);
    }

    class Buttons : ControlsConsole
    {
        public CustomButton RedrawMap { get; init; }
        public CustomButton ChangeLineAlgorithm { get; init; }
        public CustomButton ShowHideLine { get; init; }
        public CustomButton StartStopMovement { get; init; }

        public Buttons(int w, int h, string lineAlgorithmName) : base(w, h)
        {
            RedrawMap = CreateButton("1. Redraw Map", 1, Keys.D1);
            ChangeLineAlgorithm = CreateButton($"2. {lineAlgorithmName}", 3, Keys.D2);
            ShowHideLine = CreateButton("3. Hide Line", 5, Keys.D3);
            StartStopMovement = CreateButton("4. Stop Movement", 7, Keys.D4);
        }

        CustomButton CreateButton(string label, int y, Keys keyboardShortcut)
        {
            return new CustomButton(label, keyboardShortcut)
            {
                Position = Position.WithY(y),
                UseMouse = true,
                Parent = Controls,
                UseKeyboard = false,
                Text = label // has to be last
            };
        }
    }

    class CustomButton : Button
    {
        const int Padding = 4;
        public Keys KeyboardShortcut { get; init; }

        static CustomButton() =>
            Library.Default.SetControlTheme(typeof(CustomButton), new ButtonTheme());

        public CustomButton(string label, Keys keyboardShortcut) : base(label.Length + Padding, 1)
        {
            _text = "";
            KeyboardShortcut = keyboardShortcut;
        }

        public new string Text
        {
            get => _text;
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (value.Length != _text.Length)
                {
                    Resize(value.Length + Padding, 1);
                    if (Parent is ControlHost cs)
                    {
                        Position = Position.WithX(cs.ParentConsole.Surface.Width / 2 - Width / 2);
                    }
                }
                _text = value;
                IsDirty = true;
            }
        }
        public void SimulateClick()
        {
            // Fancy check to make sure Parent, Parent.Host, and Parent.Host.ParentConsole are all non-null
            if (Parent is { Host.ParentConsole: { } })
                Parent.Host.ParentConsole.IsFocused = true;

            IsFocused = true;
            InvokeClick();
            DetermineState();
        }
    }
    
    class PlayerInfo : Console
    {
        ManhattanDistance _manhattanDistance = Distance.Manhattan;
        ChebyshevDistance _chebyshevDistance = Distance.Chebyshev;
        EuclideanDistance _euclideanDistance = Distance.Euclidean;

        public static Point ReferencePoint => Point.Zero;

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
            Print("Reference point:", ReferencePoint);
            Print("BearingOfLine:", Point.BearingOfLine(player.Position).ToString("f2"));
            Print("Line length:", lineLength);
            
            // distances
            Cursor.NewLine();
            Print("EuclideanDistanceMagnitude:", distToOrigin.ToString("f2"));
            Print("Manhattan distance", _manhattanDistance.Calculate(ReferencePoint, player.Position));
            Print("Chebyshev distance", _chebyshevDistance.Calculate(ReferencePoint, player.Position));
            Print("Euclidean distance", $"{_euclideanDistance.Calculate(ReferencePoint, player.Position):f2}");
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
        readonly Grid _grid;
        readonly DeltaTime _deltaTime = new(0.25d);

        public Player Player { get; init; }

        public LineSurface Lines { get; init; }

        public Map() : base(11, 11)
        {
            // change font
            Font = Fonts.Square10;
            FontSize *= Grid.FontSizeMultiplier;

            // draw row numbers
            for (int x = 0, count = Surface.Width - 1; x < count; x++)
                Surface.SetGlyph(x + 1, 0, '0' + (x % 10), Color.LightGreen);

            // draw column numbers
            for (int y = 0, count = Surface.Height - 1; y < count; y++)
                Surface.SetGlyph(0, y + 1, '0' + (y % 10), Color.LightGreen);

            // calculate width and height for the grid and lines surfaces
            int width = Surface.Width - 1;
            int height = Surface.Height - 1;

            // create a grid
            _grid = new Grid(width, height)
            {
                Parent = this,
                Position = (1, 1)
            };

            // create a player
            Player = new(_grid.Area.Center, Math.Min(_grid.Area.Width, _grid.Area.Height));

            // create a lines layer
            Lines = new LineSurface(width, height)
            {
                Parent = this,
                FontSize = this.FontSize,
                Position = (1, 1)
            };
            Player.PositionChanged += Lines.Player_OnPositionChanged;

            // add event handler
            _deltaTime.TresholdReached += DeltaTime_OnTresholdReached;
        }

        public void Redraw()
        {
            _grid.FillWithFloorAndWalls();
            Player.Position = GlobalRandom.DefaultRNG.RandomPosition(_grid.Surface.Area, p => _grid.IsWalkable(p));
        }

        void DeltaTime_OnTresholdReached(object? sender, EventArgs args)
        {
            Direction currentDirection = Player.Direction;

            if (Player.WantsToTurn())
            {
                // try turning the player left or right
                if (!TurnPlayer())
                {
                    // try going straight ahead
                    Player.Direction = currentDirection;
                    if (_grid.IsWalkable(Player.NextPosition)) MovePlayer();
                    else
                    {
                        // dead end... turn the player back
                        Player.Direction = currentDirection + 4;
                    }
                }
            }

            // player wants to go straight
            else
            {
                // try going straight ahead
                if (_grid.IsWalkable(Player.NextPosition)) MovePlayer();

                // try turning the player left or right
                else if (!TurnPlayer())
                {
                    // dead end... turn the player back
                    Player.Direction = currentDirection + 4;
                }
            }
        }

        bool TurnPlayer()
        {
            // turn the player left or right
            Player.Turn();
            if (_grid.IsWalkable(Player.NextPosition))
            {
                MovePlayer();
                return true;
            }
            else
            {
                // turn the player the other way
                Player.InverseTurn();
                if (_grid.IsWalkable(Player.NextPosition))
                {
                    MovePlayer();
                    return true;
                }
                return false;
            }
        }

        void MovePlayer()
        {
            _grid.SetCellToFloor(Player.Position);
            Player.Walk();
            _grid.SetCellAppearance(Player.Position.X, Player.Position.Y, Player);
        }

        public override void Update(TimeSpan delta)
        {
            if (Player.IsMoving)
                _deltaTime.Add(delta);
            base.Update(delta);
        }
    }

    class LineSurface : ScreenSurface
    {
        ColoredGlyph appearance = new(Color.LightSkyBlue, Color.Transparent, 'X');
        public GoRogue.Lines.Algorithm CurrentAlgorithm;

        public LineSurface(int w, int h) : base(w, h) { }

        public int DrawLine(Point start, Point end)
        {
            Surface.Clear();
            var linePoints = GoRogue.Lines.Get(start, end, CurrentAlgorithm);
            foreach (var point in linePoints)
                if (point != start)
                    Surface.SetGlyph(point.X, point.Y, appearance);

            return linePoints.Count();
        }

        public void Player_OnPositionChanged(object? sender, PositionChangedEventArgs args)
        {
            args.LineLength = DrawLine(args.Position, PlayerInfo.ReferencePoint);
        }

        public void ChangeAlgorithm()
        {
            int i = (int) CurrentAlgorithm + 1;
            CurrentAlgorithm = Enum.IsDefined(typeof(GoRogue.Lines.Algorithm), i) ? 
                (GoRogue.Lines.Algorithm) i : (GoRogue.Lines.Algorithm) 0;
        }
    }

    class Grid : Console
    {
        public const char FloorGlyph = '.';
        public const char WallGlyph = '#';
        public const int FontSizeMultiplier = 2;

        public Grid(int w, int h) : base(w, h)
        {
            Font = Fonts.Square10;
            FontSize *= FontSizeMultiplier;
            FillWithFloorAndWalls();
        }

        public void FillWithFloorAndWalls()
        {
            Surface.Clear();

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

        public bool IsWalkable(Point position) =>
            Surface.Area.Contains(position) &&
            Surface.GetGlyph(position.X, position.Y) == FloorGlyph;
    }

    class Player : ColoredGlyph
    {
        readonly int _maxDistance;
        Point _position;
        
        public Direction Direction { get; set; }
        public int DesiredDistance { get; private set; }
        public int CurrentDistance { get; private set; }
        public bool IsMoving { get; set; } = true;

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

        public bool WantsToTurn() => CurrentDistance == DesiredDistance;

        public void Walk()
        {
            Position = NextPosition;
            CurrentDistance++;
        }

        public Point NextPosition => Position + Direction;

        void SetDesiredDistance()
        {
            DesiredDistance = GlobalRandom.DefaultRNG.NextInt(2, _maxDistance);
            CurrentDistance = 0;
        }
    }
}   