namespace SadExperiments.Games.PacMan;

static class Appearances
{
    public static ColoredGlyph Wall, Floor;

    static Appearances()
    {
        Wall = new(Color.LightBlue, Color.Transparent, 7);
        Floor = new(Color.LightGray, Color.Transparent, 3);
    }
}