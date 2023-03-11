namespace SadExperiments.Games.PacMan;

static class Appearances
{
    public static readonly ColoredGlyph Wall, Floor, Dot, PowerDot, Player;

    // level complete wall flash animation
    public static readonly Color WallFlash;

    static Appearances()
    {
        WallFlash = Color.LightGray;
        Wall = new(Color.DodgerBlue, Color.Transparent, 38); 
        Floor = new(Color.White, Color.Transparent, 39);
        Dot = new("#ffb7ae".ToColor(), Color.Transparent, 36);
        PowerDot = new("#ffb7ae".ToColor(), Color.Transparent, 37);
        Player = new(Color.Yellow, Color.Transparent, 0);
    }

    // <int neighboursAsBits, int glyphIndex>
    public static readonly Dictionary<int, int> InnerWalls = new()
    {
        // 010  
        // 010
        // 010  
        { 146, 4 },

        // 010
        // 010
        // 011
        { 147, 4 },

        // 011
        // 010
        // 010
        { 210, 4 },

        // 011
        // 010
        // 011
        { 211, 4 },

        // 011
        // 010
        // 110
        { 214, 4 },

        // 111
        // 011      buggy
        // 010
         { 474, 4 },

        // 011
        // 011
        // 011
        { 219, 4 },

        // 111
        // 011
        // 011
        { 475, 4 },

        // 011
        // 011
        // 111
        { 223, 4 },

        // 010
        // 011
        // 000
        { 152, 0 },

        // 011
        // 011
        // 000
        { 216, 0 },

        // 011
        // 011
        // 001
        { 217, 0 },

        // 111
        // 011
        // 000
        { 472, 0 },

        // 111
        // 110      offset
        // 000
        { 496, 1 },

        // 111
        // 111
        // 011
        { 507, 8 },
    };

    // <int neighboursAsBits, int glyphIndex>
    public static readonly Dictionary<int, int> PerimeterWalls = new()
    {
        // 000
        // 111
        // 000
        { 56, 12 },

        // 001
        // 111
        // 000
        { 120, 12 },

        // 100
        // 111
        // 000
        { 312, 12 },

        // 101
        // 111
        // 000
        { 376, 12 },

        // 010
        // 011
        // 000
        { 152, 16 },

        // 110
        // 111
        // 000
        { 440, 20 },

        // 011
        // 111
        // 000
        { 248, 24 },

        // 110
        // 110
        // 000
        { 432, 32 },

        // 111
        // 111
        // 000
        { 504, 40 },
    };

    // <int neighboursAsBits, int glyphIndex>
    public static readonly Dictionary<int, int> SpawnerWalls = new()
    {
        // 000
        // 110
        // 010
        { 50, 44 },

        // 000
        // 111
        // 000
        { 56, 14 },

        // 101
        // 111
        // 000
        { 376, 14 },

        // 001
        // 111
        // 000
        { 120, 14 },

        // 100
        // 111
        // 000
        { 312, 14 },
    };

    public static readonly Dictionary<int, int> SpawnerEntranceWalls = new()
    {
        // 000
        // 111
        // 000
        { 56, 48 },
    };
}