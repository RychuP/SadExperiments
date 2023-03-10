using System.IO;

namespace SadExperiments.Games.PacMan;

// game logic inspired by the article at https://pacman.holenet.info/
class Game : Page, IRestartable
{
    public const double FontSizeMultiplier = 2;
    const int LivesStart = 3;

    public static readonly Point DefaultFontSize = new Point(8, 8) * FontSizeMultiplier;
    readonly GameOverWindow _gameOverWindow = new();
    readonly Header _header = new();
    int _lives = LivesStart;
    int _level = 1;
    int _score = 0;
    Board _board;

    public Game()
    {
        #region Meta
        Title = "PacMan";
        Summary = "Work in progress.";
        Submitter = Submitter.Rychu;
        Date = new(2023, 02, 28);
        Tags = new Tag[] {Tag.SadConsole, Tag.Pixels, Tag.Game, Tag.Renderer};
        #endregion Meta

        _board = CreateBoard("Maze.txt");
        _gameOverWindow.RestartButton.Click += RestartButton_OnClick;
    }

    public void Restart()
    {
        _level = 1;
        _score = 0;
        _lives = LivesStart;
        _board = CreateBoard("Maze.txt");
        _header.Print(_lives, _score, _level);
        Children.Clear();
        Children.Add(_header, _board);
    }

    Board CreateBoard(string fileName)
    {
        var level = LoadMaze("Maze.txt");
        var board = new Board(level);
        board.IsFocused = true;
        board.DotEaten += Board_OnDotEaten;
        board.LiveLost += Board_OnLiveLost;
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

    void Board_OnDotEaten(object? o, ScoreEventArgs e)
    {
        _score += e.Value;
        _header.PrintScore(_score);
    }

    void OnGameOver()
    {
        _board.RemoveAllDots();
        _gameOverWindow.Show();
        _gameOverWindow.ShowScore(_score, _level);
        _gameOverWindow.RestartButton.IsFocused = true;
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

    static Level LoadMaze(string fileName)
    {
        // read the blueprint file (to be replaced by random generation)
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

                    case 'S':
                        start = position;
                        goto default;

                    case 'X':
                        tiles[i] = (new Spawner(position));
                        break;

                    case '-':
                        tiles[i] = (new SpawnerEntrance(position));
                        break;

                    default:
                        tiles[i] = new Floor(position);
                        break;
                }
            }
        }

        return new Level(width, height, start, tiles, dots);
    }
}

record Level(int Width, int Height, Point Start, Tile[] Tiles, List<Dot> Dots);