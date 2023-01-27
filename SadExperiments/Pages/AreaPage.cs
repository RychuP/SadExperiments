using GoRogue.Random;
using SadConsole.Quick;
using SadExperiments.UI;
using ShaiRandom.Generators;

namespace SadExperiments.Pages;

internal class AreaPage : Page
{
    // backing field for NeighborsProbeCenterPoint
    Point _neighborsProbeCenterPoint = Point.Zero;

    // backing field for AdjacencyRule
    AdjacencyRule _adjacencyRule = AdjacencyRule.EightWay;

    DisplayItems _currentDisplayItems = DisplayItems.Area;
    readonly TestArea _area;
    readonly ScreenSurface _boundsLayer;
    readonly NeighborsSurface _neighborsLayer;

    public AreaPage()
    {
        Title = "Area";
        Summary = "Generating random areas and testing perimeter functions.";

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
        var topButtons = new HorizontalButtonsConsole(Width, 1)
        {
            Parent = this,
            Position = (0, 1)
        };

        topButtons.AddButton("Change Color").Click += (o, e) =>
        {
            _area.Color = Program.RandomColor;
            ExecuteCurrentDisplayFunctions();
        };

        topButtons.AddButton("New Area").Click += (o, e) =>
        {
            _area.Regenerate();
            ExecuteCurrentDisplayFunctions();
        };

        // create bottom buttons surface
        var bottomButtons = new HorizontalButtonsConsole(Width, 1)
        {
            Parent = this,
            Position = (0, Height - 2)
        };

        bottomButtons.AddButton("Area Filled").Click += (o, e) =>
        {
            if (o is AutomatedButton button)
            {
                if ((_currentDisplayItems & DisplayItems.PerimeterPositions) == DisplayItems.PerimeterPositions)
                {
                    _currentDisplayItems ^= DisplayItems.PerimeterPositions;
                    button.Text = "Area Filled";
                }
                else
                {
                    _currentDisplayItems |= DisplayItems.PerimeterPositions;
                    button.Text = "Perimeter Positions";
                }
            }
            ExecuteCurrentDisplayFunctions();
        };

        bottomButtons.AddButton("Bounds").Click += (o, e) =>
        {
            if ((_currentDisplayItems & DisplayItems.Bounds) == DisplayItems.Bounds)
            {
                _currentDisplayItems ^= DisplayItems.Bounds;
                _boundsLayer.Surface.Clear();
            }
            else
            {
                _currentDisplayItems |= DisplayItems.Bounds;
                DrawAreaBounds(false);
            }
        };

        string adjacencyRuleLabel = "Adjacency Rule: ";
        bottomButtons.AddButton(adjacencyRuleLabel + AdjacencyRule).Click += (o, e) =>
        {
            SetNextAdjacencyRule();
            if (o is AutomatedButton b)
                b.Text = adjacencyRuleLabel + AdjacencyRule;
            ExecuteCurrentDisplayFunctions();
        };
    }

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

    public event EventHandler<NeighborsChangedEventArgs>? NeighborsChanged;

    protected void OnNeighborsChanged(NeighborsChangedEventArgs args) =>
        NeighborsChanged?.Invoke(this, args);

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
        if ((_currentDisplayItems & DisplayItems.PerimeterPositions) == DisplayItems.PerimeterPositions)
            DisplayPerimeterPositions();
        else
            DisplayArea();

        if ((_currentDisplayItems & DisplayItems.Bounds) == DisplayItems.Bounds)
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

    [Flags]
    enum DisplayItems
    {
        Area = 0,
        PerimeterPositions = 1,
        Bounds = 2
    }

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

    class TestArea : Area
    {
        readonly Rectangle _bounds;
        public Color Color { get; set; } = Color.LightGreen;

        public TestArea(Rectangle bounds)
        {
            _bounds = bounds;
            Regenerate();
        }

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
                    if ((int)Math.Abs(tempCenter.X) < horizontalRadius && (int)Math.Abs(tempCenter.Y) < verticalRadius)
                        return true;
                    else
                        return false;
                });
                Add(new Rectangle(center, horizontalRadius, verticalRadius));
            }
        }
    }

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