using System.IO;

namespace SadExperiments.Games.PacMan;

// game logic inspired by the article at https://pacman.holenet.info/
class Game : Page
{
    public const double FontSizeMultiplier = 2;
    public static readonly Point DefaultFontSize = new Point(8, 8) * FontSizeMultiplier;
    readonly Player _player = new();
    Board _board;

    public Game()
    {
        #region Meta
        Title = "PacMan";
        Summary = "Work in progress.";
        Submitter = Submitter.Rychu;
        Date = new(2023, 02, 28);
        Tags = new Tag[] {Tag.Game};
        #endregion Meta

        // create the board
        var level = LoadMaze("Maze.txt");
        _board = new Board(level, _player);
        Children.Add(_board);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Container)
            _board.IsFocused = true;
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
                        tiles[i] = new Wall(position);
                        break;

                    case '.':
                        dots.Add(new Dot(position));
                        goto default;

                    case '*':
                        dots.Add(new PowerUp(position));
                        goto default;

                    case 'A':
                        tiles[i] = new Teleport(position, 'A');
                        break;

                    case 'B':
                        tiles[i] = new Teleport(position, 'B');
                        break;

                    case 'S':
                        start = position;
                        goto default;

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