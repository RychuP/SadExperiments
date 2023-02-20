using SadCanvas;
using SadConsole.UI;
using SadRogue.Primitives;
using System.IO;

namespace SadExperiments.Games.Tetris;

class Game : Page, IRestartable
{
    #region Constants
    public static readonly int SolidGlyph = Fonts.C64.SolidGlyphIndex;
    #endregion Constants

    #region Fields
    readonly FinishedWindow _finishedWindow = new();
    readonly StartWindow _startWindow = new();
    readonly NextDisplay _nextDisplay;
    readonly InfoDisplay _infoDisplay;
    readonly Canvas _logo;
    readonly Board _board;
    readonly BorderSurface _mask;
    readonly BorderSurface _border;
    #endregion Fields

    #region Constructors
    public Game()
    {
        #region Meta
        Title = "Tetris";
        Summary = "My own implementation of the most famous game in the world.";
        Submitter = Submitter.Rychu;
        Date = new(2023, 2, 19);
        Tags = new Tag[] { Tag.SadConsole, Tag.SadCanvas, Tag.Game, Tag.Renderer };
        #endregion Meta

        // create tetris board elements
        _board = new Board() { Position = (4, 0) };
        _border = new(_board.Surface.Width + 2, _board.Surface.Height + 2)
            { Position = (_board.Position.X - 1, _board.Position.Y - 1) };

        // a mask covering the top two board rows (tetromino spawn area) shifted up by a few pixels 
        // as per guidance https://tetris.fandom.com/wiki/Tetris_Guideline
        _mask = new(_border.Surface.Width, 2)
        {
            UsePixelPositioning = true,
            Position = _border.Position.Translate(0, 1) * _border.FontSize - (0, 5) 
        };

        // add all board elements to children
        Children.Add(_border, _board, _mask);

        // calculate right column size
        int startX = _border.AbsolutePosition.X + _border.WidthPixels;
        int widthPixels = WidthPixels - startX;
        int widthCells = widthPixels / FontSize.X;

        // create tetris logo
        _logo = CreateCanvas("tetris.png");
        int remainingWidthHalved = (widthPixels - _logo.Width) / 2;
        int x = startX + remainingWidthHalved;
        int y = _mask.AbsolutePosition.Y;
        _logo.Position = (x, y);

        // create next tetromino display
        _nextDisplay = new(widthCells);
        y = _logo.Position.Y + _logo.Height + 20;
        _nextDisplay.Position = (startX, y);
        Children.Add(_nextDisplay);

        // create info display
        _infoDisplay = new InfoDisplay(widthCells);
        y = _nextDisplay.Position.Y + _nextDisplay.HeightPixels + 10;
        _infoDisplay.Position = (startX, y);
        Children.Add(_infoDisplay);

        /// TODO: change this
        Children.MoveToTop(_nextDisplay);

        // register event handlers
        _finishedWindow.RestartButton.Click += RestartButton_OnClick;
        _startWindow.StartButton.Click += StartButton_OnClick;
        _board.TetrominoPlanted += Board_OnTetrominoPlanted;
        _board.ScoreChanged += Board_OnScoreChanged;
        _board.LevelChanged += Board_OnLevelChanged;
        _board.GameOver += Board_OnGameOver;
    }
    #endregion Constructors

    #region Methods
    public void Restart()
    {
        _board.Reset();
        _infoDisplay.ShowScore(0);
        _nextDisplay.ShowNext(_board.Next);
        _startWindow.Show(true);
        _startWindow.StartButton.IsFocused = true;
    }

    Canvas CreateCanvas(string fileName)
    {
        var filePath = Path.Combine("Resources", "Images", "Tetris", fileName);
        return new Canvas(filePath)
        {
            Parent = this,
            UsePixelPositioning = true
        };
    }

    void DrawBorder(Color? color = null)
    {
        color ??= Color.Pink;
        _border.Surface.DrawOutline(color);
        for (int i = 0; i < _border.Surface.Width; i++)
            _mask.Surface.SetCellAppearance(i, 1, _border.Surface[i]);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        // reduce initial keyboard repeat delay to make the moves left and right start faster
        SadConsole.Game.Instance.Keyboard.InitialRepeatDelay = newParent is Container ? 0.3f :
            Container.Instance.DefaultInitialRepeatDelay;

        // hide all the windows when page hidden
        if (oldParent is Container)
        {
            _finishedWindow.Hide();
            _startWindow.Hide();
            _infoDisplay.RemoveInstructions();
        }

        base.OnParentChanged(oldParent, newParent);
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        // slower moves
        if (keyboard.HasKeysPressed)
        {
            if (keyboard.IsKeyPressed(Keys.X))
                _board.RotateTetrominoRight();
            else if (keyboard.IsKeyPressed(Keys.Z))
                _board.RotateTetrominoLeft();
            else if (keyboard.IsKeyPressed(Keys.Left))
                _board.MoveTetrominoLeft();
            else if (keyboard.IsKeyPressed(Keys.Right))
                _board.MoveTetrominoRight();
            else if (keyboard.IsKeyPressed(Keys.Space))
                _board.HardDropTetromino();
            else if (keyboard.IsKeyPressed(Keys.P))
                _board.TogglePause();
        }

        // faster moves
        if (keyboard.HasKeysDown)
        {
            if (keyboard.IsKeyDown(Keys.Down))
                _board.SoftDropTetromino();
        }

        return base.ProcessKeyboard(keyboard);
    }

    void Board_OnGameOver(object? o, EventArgs e)
    {
        _finishedWindow.ShowFinals(_board.Score, _board.Level);
        _finishedWindow.Show(true);
        _finishedWindow.RestartButton.IsFocused = true;
    }

    void RestartButton_OnClick(object? o, EventArgs e)
    {
        _finishedWindow.Hide();
        _board.Restart();
    }

    void Board_OnTetrominoPlanted(object? o, EventArgs e)
    {
        _nextDisplay.ShowNext(_board.Next);
    }

    void Board_OnScoreChanged(object? o, EventArgs e)
    {
        _infoDisplay.ShowScore(_board.Score);
    }

    void Board_OnLevelChanged(object? o, EventArgs e)
    {
        if (_board.Level != 0)
        {
            Color color = Color.Black;
            while (color.GetBrightness() < 0.5f)
                color = Program.RandomColor;
            DrawBorder(color);
            _infoDisplay.ShowLevel(_board.Level);
        }
        else
            DrawBorder();
    }

    void StartButton_OnClick(object? o, EventArgs e)
    {
        _startWindow.Hide();
        _board.TogglePause();
    }
    #endregion Methods
}

class BorderSurface : ScreenSurface
{
    public BorderSurface(int w, int h) : base(w, h)
    {
        Font = Fonts.Square10;
        FontSize *= 2;
    }
}