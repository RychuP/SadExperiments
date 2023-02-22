using SadConsole.Components;

namespace SadExperiments.Games.Tetris;

class Board : ScreenSurface
{
    #region Constants
    const int LinesPerLevel = 10;
    #endregion Constants

    #region Fields
    int _score = 0;
    int _level = 0;
    int _lines = 0;
    Tetromino _current = Tetromino.Next();
    readonly BlockManager _renderer = new();
    readonly Timer _timer = new(TimeSpan.Zero) { IsPaused = true };
    // gravity and line scoring values taken from classic NES tetris
    readonly int[] _lineScoring = new int[] { 40, 100, 300, 1200 };
    readonly int[] _gravity = new int[] { 48, 43, 38, 33, 28, 23, 18, 13, 
        8, 6, 5, 5, 5, 4, 4, 4, 3, 3, 3 };
    #endregion Fields

    #region Constructors
    public Board() : base(10, 22)
    {
        Font = Fonts.Square10;
        FontSize *= 2;

        Current = _current;
        SadComponents.Add(_renderer);

        _timer.TimerElapsed += Timer_OnTimerElapsed;
        SadComponents.Add(_timer);
    }
    #endregion Constructors

    #region Properties
    public Tetromino Current
    {
        get => _current;
        set
        {
            _renderer.Add(value);
            _current = value;
        }
    }

    public Tetromino Next { get; private set; } = Tetromino.Next();

    public int Level
    {
        get => _level;
        private set
        {
            _level = value;
            int gravity = _level == 29 ? 1 :
                          _level < 29 && _level >= _gravity.Length ? 2 :
                          _gravity[_level];
            var delay = TimeSpan.FromSeconds(60 / gravity);
            _timer.TimerAmount = delay;
            OnLevelChanged();
        }
    }

    public int Lines
    {
        get => _lines;
        private set
        {
            _lines = value;
            if (value % LinesPerLevel == 0)
            {
                int level = value / LinesPerLevel;
                if (level != Level)
                    Level = level;
            }
            OnLinesChanged();
        }
    }

    public int Score
    {
        get => _score;
        private set
        {
            _score = value;
            OnScoreChanged();
        }
    }
    #endregion Properties

    #region Methods
    public void RotateTetrominoLeft() => RotateTetromino(Current.RotateLeft, Current.RotateRight);
    public void RotateTetrominoRight() => RotateTetromino(Current.RotateRight, Current.RotateLeft);
    public void MoveTetrominoLeft() => MoveTetrominoHorizontally(Current.MoveLeft, Current.MoveRight);
    public void MoveTetrominoRight() => MoveTetrominoHorizontally(Current.MoveRight, Current.MoveLeft);

    public bool MoveTetrominoDown()
    {
        Current.MoveDown();

        if (!LocationIsValid())
        {
            Current.MoveUp();
            PlantCurrentTetromino();
            return false;
        }

        return true;
    }

    public void HardDropTetromino()
    {
        int score = 0;
        while (LocationIsValid())
        {
            Current.MoveDown();
            score += 2;
        }
        Current.MoveUp();
        score -= 2;

        // calculate score
        if (score > 0)
            Score += score;

        PlantCurrentTetromino();
    }

    public void SoftDropTetromino()
    {
        if (MoveTetrominoDown())
            Score += 1;
    }

    public void TogglePause()
    {
        _timer.IsPaused = !_timer.IsPaused;
    }

    public void Pause()
    {
        _timer.IsPaused = true;
    }

    public void Reset()
    {
        _timer.IsPaused = true;
        _renderer.RemoveAll();
        Tetromino.ResetBag();
        Current = Tetromino.Next();
        Next = Tetromino.Next();
        Score = 0;
        Level = 0;
        Lines = 0;
    }

    public void Restart()
    {
        Reset();
        _timer.Restart();
    }

    void Timer_OnTimerElapsed(object? o, EventArgs e)
    {
        MoveTetrominoDown();
    }

    void PlantCurrentTetromino()
    {
        if (IsGameOver())
        {
            _timer.IsPaused = true;
            OnGameOver();
        }
        else
        {
            Current = Next;
            Next = Tetromino.Next();
            RemoveFullRows();
            OnTetrominoPlanted();
        }
    }

    bool IsGameOver()
    {
        if (Current.Blocks.Any(b => b.Position.Y < 2))
            return true;
        return false;
    }

    void RemoveFullRows()
    {
        int rowsCleared = 0;

        // start at the bottom row
        int row = Surface.Height - 1;

        for (int i = 0; i < 20; i++)
        {
            // get all blocks with the row number
            var blocks = _renderer.Entities.Where(b => b.Position.Y == row).ToArray();

            // check if the row is full
            if (blocks.Length == Surface.Width)
            {
                // remove all blocks in the row
                foreach (var block in blocks)
                    _renderer.Remove(block);
                rowsCleared++;

                // move all higher rows down
                foreach (var block in _renderer.Entities)
                    if (block.Position.Y < row)
                        block.Position = block.Position.Translate(0, 1);

                // skip changing the row number
                continue;
            }

            // move to the higher row
            row--;
        }

        // calculate score
        rowsCleared = Math.Clamp(rowsCleared, 0, _lineScoring.Length);
        if (rowsCleared > 0)
        {
            Score += _lineScoring[rowsCleared - 1] * _level;
            Lines += rowsCleared;
        }
    }

    void MoveTetrominoHorizontally(Action desiredMove, Action reversedMove)
    {
        desiredMove();
        if (!LocationIsValid())
            reversedMove();
    }

    void RotateTetromino(Action desiredMove, Action reversedMove)
    {
        desiredMove();
        if (!LocationIsValid() && !KickLocationIsValid())
            reversedMove();
    }

    bool KickLocationIsValid()
    {
        Current.MoveLeft();
        if (LocationIsValid()) return true;

        Current.MoveRight();
        Current.MoveRight();
        if (LocationIsValid()) return true;

        Current.MoveLeft();
        Current.MoveDown();
        if (LocationIsValid()) return true;

        Current.MoveLeft();
        if (LocationIsValid()) return true;

        Current.MoveRight();
        Current.MoveRight();
        if (LocationIsValid()) return true;

        Current.MoveLeft();
        Current.MoveUp();
        return false;
    }

    bool LocationIsValid()
    {
        if (!CheckBlocksInBounds() || !CheckCollisionsWithOthers())
            return false;
        return true;
    }

    bool CheckCollisionsWithOthers()
    {
        _renderer.Remove(Current);
        foreach (var block in Current.Blocks)
            if (_renderer.Entities.Any(b => b.Position == block.Position))
            {
                _renderer.Add(Current);
                return false;
            }
        _renderer.Add(Current);
        return true;
    }

    bool CheckBlocksInBounds()
    {
        foreach (var block in Current.Blocks)
            if (!Surface.Area.Contains(block.Position))
                return false;
        return true;
    }

    void OnTetrominoPlanted()
    {
        TetrominoPlanted?.Invoke(this, EventArgs.Empty);
    }

    void OnGameOver()
    {
        GameOver?.Invoke(this, EventArgs.Empty);
    }

    void OnScoreChanged()
    {
        ScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    void OnLinesChanged()
    {
        LinesChanged?.Invoke(this, EventArgs.Empty);
    }

    void OnLevelChanged()
    {
        LevelChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion Methods

    #region Events
    public event EventHandler? TetrominoPlanted;
    public event EventHandler? GameOver;
    public event EventHandler? ScoreChanged;
    public event EventHandler? LinesChanged;
    public event EventHandler? LevelChanged;
    #endregion Events
}