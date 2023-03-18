﻿using System.IO;

namespace SadExperiments.Games.PacMan;

// game logic inspired by the article at https://pacman.holenet.info/
class Game : Page, IRestartable
{
    #region Fields
    public const double FontSizeMultiplier = 2d;
    public const double SpriteSpeed = 2.1d;
    public const int  MaxDifficultyLevel = 21;
    const int LivesStart = 3;
    const int LevelStart = 1;
    const int ExtraLifeScore = 7000;
    const int ExtraLifeScoreIncrease = 500;

    public static readonly Point DefaultFontSize = new Point(8, 8) * FontSizeMultiplier;
    readonly GameOverWindow _gameOverWindow = new();
    readonly Header _header = new();
    readonly string mazeFileName = "Maze";
    int _extraLifeTarget = ExtraLifeScore;
    int _lives = LivesStart;
    int _score = 0;
    Board? _board;
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

        Children.Add(_header);
        _gameOverWindow.RestartButton.Click += RestartButton_OnClick;
    }
    #endregion Constructors

    #region Properties
    public int Level { get; private set; } = LevelStart;

    public int Score
    {
        get => _score;
        set
        {
            if (_score == value) return;
            var prevScore = _score;
            _score = value;
            OnScoreChanged(prevScore, value);
        }
    }
    #endregion Properties

    #region Methods
    static Level LoadMaze(string fileName)
    {
        // read the blueprint file
        string path = Path.Combine("Resources", "Other", "PacMan", fileName);
        var text = File.ReadAllText(path);
        string lineEnd = text.Contains("\r\n") ? "\r\n" : "\n";
        //string[] lines = text.Split(Environment.NewLine);
        string[] lines = text.Split(lineEnd);
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

                    // portal need to face each other (either be on the left or right 
                    // or top and bottom walls
                    case 'A':
                        tiles[i] = new Portal(position, 'A');
                        break;
                    
                    // this is due to how the sprites set their paths
                    // after teleporting
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
        (Level, Score, _lives) = (1, 0, LivesStart);
        _header.Print(_lives, Score, Level);
        
        if (_board is not null)
            Children.Remove(_board);
        CreateBoard(mazeFileName);
        if (_board is not null)
            _board.IsFocused = true;

        _extraLifeTarget = ExtraLifeScore;
    }

    void CreateBoard(string name)
    {
        string fileName = (Level % 2) switch
        {
            0 => $"{name}2.txt",
            _ => $"{name}1.txt"
        };
        var level = LoadMaze(fileName);
        _board = new Board(level, this);
        _board.DotEaten += Board_OnDotEaten;
        _board.GhostEaten += Board_OnGhostEaten;
        _board.LiveLost += Board_OnLiveLost;
        _board.LevelComplete += Board_OnLevelComplete;
    }

    void Board_OnLiveLost(object? o, EventArgs e)
    {
        if (--_lives == 0)
            OnGameOver();

        else if (_lives > 0)        
        {
            _header.PrintLives(_lives);
            _board?.Restart();
        }
    }

    void Board_OnDotEaten(object? o, DotEventArgs e)
    {
        Score += e.Dot.Value;
    }

    void Board_OnGhostEaten(object? o, GhostEventArgs e)
    {
        Score += e.Value;
    }

    void Board_OnLevelComplete(object? o, EventArgs e)
    {
        if (_board is not null)
            Children.Remove(_board);
        _header.PrintLevel(++Level);
        CreateBoard(mazeFileName);
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

    void OnScoreChanged(int prevScore, int newScore)
    {
        _header.PrintScore(newScore);

        if (newScore >= _extraLifeTarget)
        {
            Sounds.ExtraLife.Play();
            _lives++;
            _extraLifeTarget += ExtraLifeScore + Level * ExtraLifeScoreIncrease;
        }
    }

    void OnGameOver()
    {
        Sounds.StopAll();
        _board?.RemoveDots();
        _gameOverWindow.Show();
        _gameOverWindow.ShowScore(Score, Level);
        _gameOverWindow.RestartButton.IsFocused = true;
    }
    #endregion Methods
}

record Level(int Width, int Height, Point Start, Tile[] Tiles, List<Dot> Dots);