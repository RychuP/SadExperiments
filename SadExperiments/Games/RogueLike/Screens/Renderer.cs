using GoRogue.GameFramework;
using GoRogue.SpatialMaps;
using SadExperiments.Games.RogueLike.World;
using SadExperiments.Games.RogueLike.World.Entities;
using SadRogue.Primitives.GridViews;

namespace SadExperiments.Games.RogueLike.Screens;

// displays the dungeon
internal class Renderer : ScreenSurface
{
    #region Fields
    const int FontSizeX = 12;
    const int FontSizeY = 12;
    const int StandardFontWidthPixels = Program.Width * 8;
    const int StandardFontHeightPixels = (Program.Height - StatusPanel.Height) * 16;
    public const int Width = StandardFontWidthPixels / FontSizeX;
    public const int Height = StandardFontHeightPixels / FontSizeY;
    #endregion

    #region Constructors
    public Renderer(Dungeon dungeon) : base(Width, Height)
    {
        Font = Fonts.Cheepicus12;

        int x = (StandardFontWidthPixels - WidthPixels) / 2;
        int y = (StandardFontHeightPixels - HeightPixels) / 2;
        UsePixelPositioning = true;
        Position = (x, y);

        dungeon.ObjectMoved += Dungeon_OnObjectMoved;
        dungeon.MapGenerated += Dungeon_OnMapGenerated;
        dungeon.ObjectRemoved += Dungeon_OnObjectRemoved;
        dungeon.ObjectAdded += Dungeon_OnObjectAdded;
    }
    #endregion

    #region Properties

    #endregion

    #region Methods
    void DrawTerrain(Dungeon dungeon)
    {
        foreach (var point in dungeon.View.Positions())
        {
            var (x, y) = point - dungeon.View.Position;
            var bg = Color.Transparent;
            if (dungeon.GetTerrainAt(point) is Terrain terrain && dungeon.PlayerExplored[point])
                bg = dungeon.PlayerFOV.CurrentFOV.Contains(point) ? terrain.LightColor : terrain.DarkColor;
            Surface.SetCellAppearance(x, y, new ColoredGlyph(Color.Transparent, bg, 0));
        }
    }

    void DrawEntityAtPos(Dungeon dungeon, Point position)
    {
        var (x, y) = position - dungeon.View.Position;
        if (!Surface.IsValidCell(x, y)) return;

        // empty appearance
        int glyph = 0;
        Color fg = Color.Transparent;

        // get the top most entity
        var entity = dungeon.GetEntityAt<Entity>(position);

        // entity appearance
        if (entity is not null)
        {
            glyph = entity.Glyph;
            fg = entity.Color;
        }

        // set appearance
        Surface.SetForeground(x, y, fg);
        Surface.SetGlyph(x, y, glyph);
    }

    void DrawEntities(Dungeon dungeon)
    {
        foreach (var point in dungeon.PlayerFOV.CurrentFOV)
            DrawEntityAtPos(dungeon, point);
    }

    void DrawDungeon(Dungeon dungeon)
    {
        DrawTerrain(dungeon);
        DrawEntities(dungeon);
    }
    #endregion

    #region Event Handlers
    void Dungeon_OnObjectMoved(object? o, ItemMovedEventArgs<IGameObject> e)
    {
        if (o is not Dungeon dungeon) return;

        // draw everything
        if (e.Item is Player)
            DrawDungeon(dungeon);
        
        // draw entity only
        else
        {
            DrawEntityAtPos(dungeon, e.OldPosition);
            DrawEntityAtPos(dungeon, e.NewPosition);
        }
    }

    void Dungeon_OnObjectRemoved(object? o, ItemEventArgs<IGameObject> e)
    {
        if (o is not Dungeon dungeon) return;
        DrawEntityAtPos(dungeon, e.Position);
    }

    void Dungeon_OnObjectAdded(object? o, ItemEventArgs<IGameObject> e)
    {
        if (o is not Dungeon dungeon) return;
        DrawEntityAtPos(dungeon, e.Position);
    }

    void Dungeon_OnMapGenerated(object? o, EventArgs e)
    {
        if (o is not Dungeon dungeon) return;
        DrawDungeon(dungeon);
    }
    #endregion

    #region Events

    #endregion
}