namespace SadExperiments.Games.PacMan;

static class Appearances
{
    public static readonly ColoredGlyph Wall, Floor;

    static Appearances()
    {
        Wall = new(Color.LightBlue, Color.Transparent, 38);
        Floor = new(Color.LightGray, Color.Transparent, 36);
    }

    // <int neighboursAsBits, int glyphIndex>
    public static readonly Dictionary<int, int> InnerWalls = new()
    {
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

        // 010
        // 011
        // 000
        { 152, 16 },
    };
}