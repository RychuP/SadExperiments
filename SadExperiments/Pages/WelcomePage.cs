using SadConsole.Ansi;
using System.IO;

namespace SadExperiments.Pages;

class WelcomePage : Page
{
    public WelcomePage()
    {
        #region Meta
        Title = "Sad Experiments";
        Summary = "Small projects based on SadConsole and related libraries.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Animations, Tag.Decorators, Tag.ImageConversion };
        #endregion Meta

        var ansi = new TreeBranch();
        ansi.Position = (Surface.Width - ansi.Surface.View.Width, 0);

        var logo = new SadExperimentsLogo();
        logo.Position = (0, HeightPixels - logo.HeightPixels);

        Children.Add(logo, ansi);

        int currentRow = PrintPrompts(3, new string[] {
            "F1 - Previous page",
            "F2 - Next page",
            "F3 - List of contents",
            "F4 - Color picker",
            "F5 - Character picker",
        }, Color.White);

        // keyboard shortcut decorations
        for (int i = 0; i < Surface.Count; i++)
        {
            if (Surface[i].Glyph == 'F')
            {
                Surface[i].Foreground = Color.LightGreen;
                Surface[i + 1].Foreground = Color.LightGreen;
            }
        }
    }

    // prints centered prompts starting at the given row
    int PrintPrompts(int row, string[] prompts, Color color)
    {
        int spacer = 2;
        row -= spacer;
        Array.ForEach(prompts, t => Surface.Print(3, row += spacer, t, color));
        return row;
    }
}

class TreeBranch : ScreenSurface
{
    public TreeBranch() : base(47, 23, 80, 35)
    {
        string path = Path.Combine("Resources", "Other", "LDA-FALL.ANS");
        Document doc = new(path);
        AnsiWriter writer = new(doc, Surface);
        writer.ReadEntireDocument();
        Surface.View = Surface.View.ChangePosition(new Point(30, 0));

        var color = "#00aaaa".ToColor();
        foreach (var cg in Surface)
        {
            if (cg.Foreground.GetBrightness() < 0.2)
                cg.Foreground = Color.DarkSeaGreen;
            if (cg.Background == color)
                cg.Background = Color.Black;
        }

        Surface.Print(66, 19, "\"Fall\"", Color.DarkGray);
        Surface.Print(66, 20, "by LDA", Color.DarkGray);
    }
}

class SadExperimentsLogo : ScreenSurface
{
    public SadExperimentsLogo() : base(1, 16)
    {
        FontSize *= 0.75d;
        UsePixelPositioning = true;
        Surface.SetDefaultColors(Color.White, Color.Black);
    }

    protected override void OnParentChanged(IScreenObject oldParent, IScreenObject newParent)
    {
        if (newParent is Page page && Surface is ICellSurfaceResize surface)
        {
            int width = page.WidthPixels / FontSize.X + 1;
            surface.Resize(width, Surface.Height, width, Surface.Height, false);
            Surface.PrintTheDraw(3, 0, "Sad", Fonts.Destruct);
            Surface.PrintTheDraw(10, 8, "Experiments", Fonts.Destruct);
        }
        base.OnParentChanged(oldParent, newParent);
    }
}