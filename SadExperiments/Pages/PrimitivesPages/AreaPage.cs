using GoRogue.Random;
using SadConsole.Quick;
using SadExperiments;
using SadExperiments.UI;
using ShaiRandom.Generators;

namespace SadExperiments.Pages.PrimitivesPages;

internal class AreaPage : Page
{
    // backing field for NeighborsProbeCenterPoint
    Point _neighborsProbeCenterPoint = Point.Zero;

    // backing field for AdjacencyRule
    AdjacencyRule _adjacencyRule = AdjacencyRule.EightWay;

    HorizontalButtonsConsole _topButtons;
    HorizontalButtonsConsole _bottomButtons;

    DisplayItems _currentDisplayItems = DisplayItems.Area;
    readonly TestArea _area;
    readonly ScreenSurface _boundsLayer;
    readonly NeighborsSurface _neighborsLayer;

    public AreaPage()
    {
        Title = "Area";
        Summary = "Generating random areas and testing perimeter functions.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Primitives, Tag.Area, Tag.UI };

        // create area
        _area = new TestArea(Surface.Area);

        // create a surface for drawing area bounds
        _boundsLayer = new ScreenSurface(Width, Height) { Parent = this };

        // create a surface for drawing neighbors
        _neighborsLayer = new NeighborsSurface(Width, Height) { Parent = this };
        NeighborsChanged += _neighborsLayer.Page_OnNeighborsChanged;
        _neighborsLayer.WithMouse((o, m) => ProcessMouse(m));

        // display area
        ExecuteCurrentDisplayFunctions();

        // create top buttons surface
        _topButtons = new HorizontalButtonsConsole(Width, 1)
        {
            Parent = this,
            Position = (0, 1)
        };
        _topButtons.WithKeyboard((o, k) => ProcessKeyboard(k));

        _topButtons.AddButton("Change Color", Keys.D1).Click += (o, e) =>
        {
            _area.Color = Program.RandomColor;
            ExecuteCurrentDisplayFunctions();
        };

        _topButtons.AddButton("New Area", Keys.D2).Click += (o, e) =>
        {
            _area.Regenerate();
            ExecuteCurrentDisplayFunctions();
        };

        // create bottom buttons surface
        _bottomButtons = new HorizontalButtonsConsole(Width, 1)
        {
            Parent = this,
            Position = (0, Height - 2)
        };
        _bottomButtons.WithKeyboard((o, k) => ProcessKeyboard(k));

        _bottomButtons.AddButton("Area Filled", Keys.D3).Click += (o, e) =>
        {
            if (o is VariableWidthButton button)
            {
                if (_currentDisplayItems.HasFlag(DisplayItems.PerimeterPositions))
                {
                    _currentDisplayItems.UnsetFlag(DisplayItems.PerimeterPositions);
                    button.Text = "Area Filled";
                }
                else
                {
                    _currentDisplayItems.SetFlag(DisplayItems.PerimeterPositions);
                    button.Text = "Perimeter Positions";
                }
            }
            ExecuteCurrentDisplayFunctions();
        };

        _bottomButtons.AddButton("Bounds", Keys.D4).Click += (o, e) =>
        {
            if (_currentDisplayItems.HasFlag(DisplayItems.Bounds))
            {
                _currentDisplayItems.UnsetFlag(DisplayItems.Bounds);
                _boundsLayer.Surface.Clear();
            }
            else
            {
                _currentDisplayItems.SetFlag(DisplayItems.Bounds);
                DrawAreaBounds(false);
            }
        };

        string adjacencyRuleLabel = "Adjacency Rule ";
        _bottomButtons.AddButton(adjacencyRuleLabel + AdjacencyRule, Keys.D5).Click += (o, e) =>
        {
            SetNextAdjacencyRule();
            if (o is VariableWidthButton b)
                b.Text = adjacencyRuleLabel + AdjacencyRule;
            ExecuteCurrentDisplayFunctions();
        };
    }

    /// <summary>
    /// Generates event args for the <see cref="NeighborsChanged"/> event.
    /// </summary>
    NeighborsChangedEventArgs NCEventArgs
    {
        get
        {
            var neighbors = AdjacencyRule.Neighbors(NeighborsProbeCenterPoint);
            neighbors = neighbors.Intersect(_area);
            NeighborsChangedEventArgs args = new(neighbors, NeighborsProbeCenterPoint);
            return args;
        }
    }

    public AdjacencyRule AdjacencyRule
    {
        get => _adjacencyRule;
        set
        {
            if (_adjacencyRule == value) return;
            _adjacencyRule = value;
            OnNeighborsChanged(NCEventArgs);
        }
    }

    /// <summary>
    /// Point on the area use for finding neighbors.
    /// </summary>
    public Point NeighborsProbeCenterPoint
    {
        get => _neighborsProbeCenterPoint;
        set
        {
            if (_neighborsProbeCenterPoint == value) return;
            _neighborsProbeCenterPoint = value;
            OnNeighborsChanged(NCEventArgs);
        }
    }

    /// <summary>
    /// Event fired when neighbors of the probe change.
    /// </summary>
    public event EventHandler<NeighborsChangedEventArgs>? NeighborsChanged;

    /// <summary>
    /// Invokes the <see cref="NeighborsChanged"/> event.
    /// </summary>
    /// <param name="args"></param>
    protected void OnNeighborsChanged(NeighborsChangedEventArgs args) =>
        NeighborsChanged?.Invoke(this, args);

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        // check for keyboard shortcuts invoking button clicks when the buttons consoles are not focused
        if (!_topButtons.IsFocused && _topButtons.KeyboardShortcutPressed(keyboard))
            return true;
        if (!_bottomButtons.IsFocused && _bottomButtons.KeyboardShortcutPressed(keyboard))
            return true;
        return base.ProcessKeyboard(keyboard);
    }

    public override bool ProcessMouse(MouseScreenObjectState state)
    {
        if (_area.Contains(state.CellPosition))
        {
            // this needs to be checked separately from the above
            if (NeighborsProbeCenterPoint != state.CellPosition)
                NeighborsProbeCenterPoint = state.CellPosition;
        }
        else if (NeighborsProbeCenterPoint != Surface.Area.Center)
            NeighborsProbeCenterPoint = Surface.Area.Center;

        return base.ProcessMouse(state);
    }

    void DrawAreaBounds(bool clear = true)
    {
        if (clear)
            _boundsLayer.Surface.Clear();
        _boundsLayer.Surface.DrawRectangle(_area.Bounds, Color.Yellow, 176);
    }

    void SetNextAdjacencyRule()
    {
        int i = (int)AdjacencyRule.Type + 1;
        AdjacencyRule = Enum.IsDefined(typeof(AdjacencyRule.Types), i) ? (AdjacencyRule.Types)i : 0;
    }

    void ExecuteCurrentDisplayFunctions()
    {
        if (_currentDisplayItems.HasFlag(DisplayItems.PerimeterPositions))
            DisplayPerimeterPositions();
        else
            DisplayArea();

        if (_currentDisplayItems.HasFlag(DisplayItems.Bounds))
            DrawAreaBounds();
    }

    void DisplayPerimeterPositions()
    {
        Surface.Clear();
        var perimeterPositions = _area.PerimeterPositions(_adjacencyRule);
        foreach (var point in perimeterPositions)
            Surface.SetBackground(point.X, point.Y, _area.Color);
    }

    void DisplayArea()
    {
        Surface.Clear();
        for (int i = 0; i < _area.Count; i++)
        {
            Point p = _area[i];
            Surface.SetBackground(p.X, p.Y, _area.Color);
        }
    }

    /// <summary>
    /// List of possible items that can be displayed on the screen.
    /// </summary>
    [Flags]
    public enum DisplayItems
    {
        Area = 0,
        PerimeterPositions = 1,
        Bounds = 2
    }

    /// <summary>
    /// Surface used for displaying neighbors and the probe.
    /// </summary>
    class NeighborsSurface : ScreenSurface
    {
        readonly ColoredGlyph _neighborGlyph;
        readonly ColoredGlyph _centerGlyph;
        readonly Color _fgColor = Color.Red;

        public NeighborsSurface(int w, int h) : base(w, h)
        {
            _neighborGlyph = new ColoredGlyph(_fgColor, Color.Transparent, 'X');
            _centerGlyph = new ColoredGlyph(_fgColor, Color.White, ' ');
        }

        public void Page_OnNeighborsChanged(object? obj, NeighborsChangedEventArgs args)
        {
            Surface.Clear();
            foreach (var point in args.Neighbors)
                Surface.SetGlyph(point.X, point.Y, _neighborGlyph);
            Surface.SetGlyph(args.Center.X, args.Center.Y, _centerGlyph);
        }
    }

    /// <summary>
    /// Area displayed in the center of the screen.
    /// </summary>
    class TestArea : Area
    {
        readonly Rectangle _bounds;
        public Color Color { get; set; } = Color.LightGreen;

        public TestArea(Rectangle bounds)
        {
            _bounds = bounds;
            Regenerate();
        }

        /// <summary>
        /// Generates a new set of points for the area.
        /// </summary>
        public void Regenerate()
        {
            Remove(this);
            int rectCount = GlobalRandom.DefaultRNG.NextInt(10, 20);
            for (int i = 0; i < rectCount; i++)
            {
                int horizontalRadius = GlobalRandom.DefaultRNG.NextInt(2, _bounds.Width / 4 - 2);
                int verticalRadius = GlobalRandom.DefaultRNG.NextInt(2, _bounds.Height / 4 - 2);
                Point center = GlobalRandom.DefaultRNG.RandomPosition(_bounds, p =>
                {
                    var tempCenter = _bounds.Center - p;
                    if (Math.Abs(tempCenter.X) < horizontalRadius && Math.Abs(tempCenter.Y) < verticalRadius)
                        return true;
                    else
                        return false;
                });
                Add(new Rectangle(center, horizontalRadius, verticalRadius));
            }
        }
    }

    /// <summary>
    /// Arguments passed around when the neighbors change.
    /// </summary>
    public class NeighborsChangedEventArgs : EventArgs
    {
        public IEnumerable<Point> Neighbors { get; init; }
        public Point Center { get; init; }
        public NeighborsChangedEventArgs(IEnumerable<Point> neighbors, Point center)
        {
            Neighbors = neighbors;
            Center = center;
        }
    }
}

static class DisplayItemsExtensions
{
    public static void UnsetFlag(ref this AreaPage.DisplayItems displayItems, AreaPage.DisplayItems item) =>
        displayItems ^= item;

    public static void SetFlag(ref this AreaPage.DisplayItems displayItems, AreaPage.DisplayItems item) =>
        displayItems |= item;
}