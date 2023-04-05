using SadExperiments.Games.RogueLike.World;
using SadExperiments.Games.RogueLike.Screens;

namespace SadExperiments.Games.RogueLike;

internal class Game : Page, IRestartable
{
    #region Fields
    // world
    readonly Dungeon _dungeon = new();

    // screens
    readonly Renderer _renderer;
    readonly StatusPanel _statusPanel;
    readonly InfoPanel _infoPanel;
    readonly InventoryPanel _inventoryPanel;
    #endregion 

    #region Constructors
    public Game()
    {
        #region Meta
        Title = "RogueLike Dungeon";
        Summary = "Unintegrated version of the SadRogue_Integration example game.";
        Submitter = Submitter.Rychu;
        Date = new(2023, 04, 02);
        Tags = new Tag[] { Tag.SadConsole, Tag.GoRogue, Tag.Game };
        #endregion Meta

        // renderer
        _renderer = new(_dungeon);

        // screens
        _infoPanel = new(_dungeon);
        _statusPanel = new(_dungeon);
        _inventoryPanel = new(_dungeon.Player);
        Children.Add(_renderer, _statusPanel, _infoPanel, _inventoryPanel);
    }
    #endregion

    #region Methods
    public void Restart()
    {
        _infoPanel.Reset();
        _statusPanel.Reset();
        _inventoryPanel.Reset();
        _dungeon.Reset();
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.HasKeysPressed)
        {
            if (_dungeon.Player.IsAlive())
                _dungeon.ProcessKeyboard(keyboard);
            else if (keyboard.IsKeyPressed(Keys.Enter))
                Restart();
                
        }
        return base.ProcessKeyboard(keyboard);
    }
    #endregion

    #region Event Handlers
    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Container)
            GameHost.Instance.Keyboard.InitialRepeatDelay = 0.3f;
        else if (oldParent is Container)
            GameHost.Instance.Keyboard.InitialRepeatDelay = Container.Instance.DefaultInitialRepeatDelay;
        base.OnParentChanged(oldParent, newParent);
    }
    #endregion
}