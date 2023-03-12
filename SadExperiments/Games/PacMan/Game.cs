using System.IO;

namespace SadExperiments.Games.PacMan;

// game logic inspired by the article at https://pacman.holenet.info/
class Game : Page, IRestartable
{
    #region Fields
    public const double FontSizeMultiplier = 2d;
    public const double SpriteSpeed = 2d;
    public const int MaxDifficultyLevel = 21;
    const int LivesStart = 3;
    const int LevelStart = 2;

    public static readonly Point DefaultFontSize = new Point(8, 8) * FontSizeMultiplier;
    readonly GameOverWindow _gameOverWindow = new();
    readonly Header _header = new();
    int _lives = LivesStart;
    int _score = 0;
    Board _board;
    readonly string mazeFileName = "Maze.txt";
    #endregion Fields

    #region Constructors
    public Game()
    {
        #region Meta
        Title = "PacMan";
        Summary = "Work in progress.";
        Submitter = Submitter.Rychu;
        Date = new(2023, 02, 28);
        Tags = new Tag[] {Tag.SadConsole, Tag.Pixels, Tag.Game, Tag.Renderer};
        #endregion Meta

        _board = CreateBoard(mazeFileName);
        _gameOverWindow.RestartButton.Click += RestartButton_OnClick;
    }
    #endregion Constructors

    #region Properties
    public int Level { get; private set; } = LevelStart;
    #endregion Properties

    #region Methods
    static Level LoadMaze(string fileName)
    {
        // read the blueprint file
        string path = Path.Combine("Resources", "Other", "PacMan", fileName);
        var text = File.ReadAllText(path);
        string[] lines = text.Split("\r\n");
        int width = lines[0].Length;
        int height = lines.Length;

        // create tiles
        var tiles = new Tile[width * height];
        var dots = new List<Dot>(tiles.Length * 3 / 4);
        Point start = Point.Zero;
        for (int y = 0, i = 0; y < height; y++)
        {
            string line = lines[y];
            for (int x = 0; x < width; x++, i++)
            {
                char symbol = line[x];
                Point position = (x, y);

                switch (symbol)
                {
                    case 'S':
                        start = position;
                        goto default;

                    case '#':
                        tiles[i] = (x == 0 || y == 0 || x == width - 1 || y == height - 1) ?
                            new PerimeterWall(position) : new Wall(position);
                        break;

                    case '.':
                        dots.Add(new Dot(position));
                        goto default;

                    case '*':
                        dots.Add(new PowerDot(position));
                        goto default;

                    case 'A':
                        tiles[i] = new Portal(position, 'A');
                        break;

                    case 'B':
                        tiles[i] = new Portal(position, 'B');
                        break;

                    case 'X':
                        tiles[i] = new Spawner(position);
                        break;

                    case '-':
                        tiles[i] = new SpawnerEntrance(position);
                        break;

                    default:
                        tiles[i] = new Floor(position);
                        break;
                }
            }
        }

        return new Level(width, height, start, tiles, dots);
    }
    public void Restart()
    {
        Level = 1;
        _score = 0;
        _lives = LivesStart;
        _board = CreateBoard(mazeFileName);
        _header.Print(_lives, _score, Level);
        Children.Clear();
        Children.Add(_header, _board);
    }

    Board CreateBoard(string fileName)
    {
        var level = LoadMaze(fileName);
        var board = new Board(level);
        board.IsFocused = true;
        board.DotEaten += Board_OnDotEaten;
        board.GhostEaten += Board_OnGhostEaten;
        board.LiveLost += Board_OnLiveLost;
        board.LevelComplete += Board_OnLevelComplete;
        return board;
    }

    void Board_OnLiveLost(object? o, EventArgs e)
    {
        if (--_lives == 0)
            OnGameOver();
        else
        {
            _header.PrintLives(_lives);
            _board.Restart();
        }
    }

    void Board_OnDotEaten(object? o, DotEventArgs e)
    {
        _score += e.Dot.Value;
        _header.PrintScore(_score);
    }

    void Board_OnGhostEaten(object? o, ScoreEventArgs e)
    {
        _score += e.Value;
        _header.PrintScore(_score);
    }

    void Board_OnLevelComplete(object? o, EventArgs e)
    {
        _header.PrintLevel(++Level);

        // due to OnParentChanged and OnGameStart sprite events, every level a new board needs to be created
        Children.Remove(_board);
        _board = CreateBoard(mazeFileName);
        Children.Add(_board);
    }

    void RestartButton_OnClick(object? o, EventArgs e)
    {
        _gameOverWindow.Hide();
        Restart();
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        _gameOverWindow.Hide();
        if (oldParent is Container)
            Sounds.StopAll();
        base.OnParentChanged(oldParent, newParent);
    }

    void OnGameOver()
    {
        Sounds.StopAll();
        _board.RemoveDots();
        _gameOverWindow.Show();
        _gameOverWindow.ShowScore(_score, Level);
        _gameOverWindow.RestartButton.IsFocused = true;
    }
    #endregion Methods
}

record Level(int Width, int Height, Point Start, Tile[] Tiles, List<Dot> Dots);