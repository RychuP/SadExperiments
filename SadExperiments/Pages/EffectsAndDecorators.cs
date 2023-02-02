using SadConsole.Effects;

namespace SadExperiments.Pages;

internal class EffectsAndDecorators : Page
{
    public EffectsAndDecorators()
    {
        Title = "Effects And Decorators";
        Summary = "Testing different methods of applying effects and decorators.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.CellDecorator, Tag.Color, Tag.Effects, Tag.Decorators };

        var bgColor = Color.AnsiBlackBright.GetDarker();
        DefaultBackground = bgColor;
        this.Clear();

        // resize the font for better clarity
        FontSize = new Point(20, 40);

        // create a player glyph
        var top = new CellDecorator(Color.AnsiCyan, 0x7e, Mirror.None);
        var bottom = new CellDecorator(Color.AnsiGreen, 0x7e, Mirror.Vertical);
        var left = new CellDecorator(Color.Blue, 0xaa, Mirror.Horizontal);
        var right = new CellDecorator(Color.Red, 0xaa, Mirror.None);
        CellDecorator[] decorators = { top, bottom, left, right };
        var playerGlyph = new ColoredGlyph(Color.White, bgColor, 1, Mirror.None, true, decorators);

        // place the glyph on the surface
        this.SetCellAppearance(2, 4, playerGlyph);

        // different methods of copying appearance
        this.SetCellAppearance(4, 4, this[2, 4]);
        this.SetCellAppearance(6, 4, this.GetCellAppearance(2, 4));
        this[2, 4].CopyAppearanceTo(this[8, 4]);
        this[10, 4].CopyAppearanceFrom(this[2, 4]);
        this[12, 4].CopyAppearanceFrom(this[2, 4]);

        // apply blink effect
        Blink blink = new();
        this.SetEffect(2, 4, blink);

        // create a fade effect
        Fade fade = new()
        {
            FadeBackground = true,
            UseCellBackground = false,
            DestinationBackground = new Gradient(Color.Green, Color.Red, Color.Blue),
            FadeDuration = TimeSpan.FromSeconds(3),
            CloneOnAdd = true,
            Repeat = true,
            AutoReverse = true,
            StartDelay = TimeSpan.FromSeconds(1)
        };

        // apply fade effect
        this.SetEffect(4, 4, fade);
        ClearDecorators(this[4, 4]);

        // apply fade effect to a neighbour
        this.SetEffect(6, 4, fade);
        ClearDecorators(this[6, 4]);

        // modify fade on the neighbour
        var clonedFade = this.GetEffect(6, 4) as Fade;
        if (clonedFade is null) return;
        clonedFade.UseCellBackground = true;
        clonedFade.DestinationBackground = new Gradient(Color.Green, Color.Blue, Color.Red);
        clonedFade.FadeForeground = true;

        // create a blink glyph effect
        BlinkGlyph blinkGlyph = new()
        {
            GlyphIndex = 0x05,
            CloneOnAdd = true,
            StartDelay = TimeSpan.FromSeconds(0.5d)
        };

        // apply the blink glyph effect
        this.SetEffect(8, 4, blinkGlyph);
        ClearDecorators(this[8, 4]);

        // create a chain of effects
        EffectSet effectSet = new()
        {
            Repeat = true,
            DelayBetweenEffects = TimeSpan.FromSeconds(1)
        };

        effectSet.Effects.AddLast(new Fade
        {
            FadeBackground = true,
            UseCellBackground = true,
            UseCellDestinationReverse = false,
            DestinationBackground = new Gradient(Color.White, Color.Green, Color.Yellow),
            FadeDuration = TimeSpan.FromSeconds(4),
            Repeat = false,
            AutoReverse = true
        });

        effectSet.Effects.AddLast(new Fade
        {
            FadeForeground = true,
            UseCellForeground = false,
            UseCellDestinationReverse = false,
            DestinationForeground = new Gradient(Color.Green, Color.AnsiRed),
            FadeDuration = TimeSpan.FromSeconds(4),
            Repeat = false,
            AutoReverse = true
        });

        effectSet.Effects.AddLast(new Fade
        {
            FadeBackground = true,
            UseCellBackground = true,
            UseCellDestinationReverse = false,
            DestinationBackground = new Gradient(Color.White, Color.Blue, Color.Red),
            FadeDuration = TimeSpan.FromSeconds(4),
            Repeat = false,
            AutoReverse = true
        });

        effectSet.Effects.AddLast(new Fade
        {
            FadeForeground = true,
            UseCellForeground = true,
            UseCellDestinationReverse = false,
            DestinationForeground = new Gradient(Color.Green, Color.AnsiBlue),
            FadeDuration = TimeSpan.FromSeconds(4),
            Repeat = false,
            AutoReverse = true
        });

        // apply chain
        this.SetEffect(10, 4, effectSet);
        ClearDecorators(this[10, 4]);
        if (this.GetEffect(10, 4) is EffectSet es) es.Restart();

        static void ClearDecorators(ColoredGlyph cg)
        {
            cg.Decorators = Array.Empty<CellDecorator>();
        }
    }
}