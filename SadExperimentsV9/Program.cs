using System;
using System.Collections.Generic;
using SadConsole;
using SadConsole.Input;
using SadConsole.Effects;
using SadRogue.Primitives;
using SadConsole.Instructions;
using SadExperimentsV9.Consoles;
using Console = SadConsole.Console;

namespace SadExperiments
{
    public static class Program
    {
        public static int Width = 80;
        public static int Height = 30;

        static void Main()
        {
            Settings.WindowTitle = "SadConsole Experiments";
            // Settings.ResizeMode = Settings.WindowResizeOptions.Scale;

            // Setup the engine and create the main window.
            Game.Create(Width, Height);
            // Game.Instance.ToggleFullScreen();

            // Hook the start event so we can add consoles to the system.
            Game.Instance.OnStart += InitTestComponentUpdateAndRender;
            //Game.Instance.DefaultFontSize = IFont.Sizes.Two;

            // Start the game.
            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        #region Inits

        // testing keyboard KeysDown, KeysPressed and KeysReleased processing
        static void InitVisualisationOfKeyboardProcessing()
        {
            Test(new KeyboardProcessing(Width, Height));
        }

        // testing both Render and Update steps over a period of one second to see how many times they get called
        static void InitTestComponentUpdateAndRender()
        {
            Test(new UpdateAndRenderDifference(Width, Height));
        }

        // testing a simple rectangle manipulation
        static void InitRectangleManipulation()
        {
            var sc = Game.Instance.StartingConsole;

            ColoredGlyph glyph = new ColoredGlyph(Color.Violet, Color.Black, 177);
            Rectangle r = new(2, 2, 10, 5);

            // draw the first box and display some info about it
            sc.DrawBox(r, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThick, glyph));
            sc.Print(2, 10, "Original rectangle position and size:");
            sc.Print(2, 11, r.Position.ToString());
            sc.Print(2, 12, r.Size.ToString());

            // expand the rectangle by 1 on each side;
            var r2 = r.Expand(1, 1);

            // print some info about the expanded box
            sc.Print(2, 14, "Expanded (by 1 on each side) rectangle position and size:");
            sc.Print(2, 15, r2.Position.ToString());
            sc.Print(2, 16, r2.Size.ToString());

            // move the expanded box right and draw
            glyph.Foreground = Color.Yellow;
            var r3 = r2.ChangeX(15);
            sc.DrawBox(r3, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThick, glyph));
        }

        // another movable character in a colorful, checkered room
        static void InitMovableCharacter2()
        {
            var map = new CheckeredRoom();
        }
        
        static void InitSmoothScrollingConsole()
        {
            var c = new Console(Width, Height, Width * 4, Height * 4);
            c.FillWithRandomGarbage(c.Font);
            SadConsole.Quick.Keyboard.WithKeyboard(c, (host, k) =>
            {
                if (host is Console console && k.HasKeysDown)
                {
                    if (k.IsKeyDown(Keys.Left))
                    {
                        console.ViewPosition += Direction.Left;
                    }

                    else if (k.IsKeyDown(Keys.Right))
                    {
                        console.ViewPosition += Direction.Right;
                    }

                    if (k.IsKeyDown(Keys.Up))
                    {
                        console.ViewPosition += Direction.Up;
                    }

                    else if (k.IsKeyDown(Keys.Down))
                    {
                        console.ViewPosition += Direction.Down;
                    }
                    return true;
                }
                return false;
            });
            ReplaceSC(c);
            c.IsFocused = true;
        }

        static void InitInstructions()
        {
            int gradientPositionX = -50, gradientChange = 1, angle = 45; //angleChange = 15;
            var logoText = new ColorGradient(new[] { Color.Magenta, Color.Yellow }, new[] { 0.0f, 1f })
                               .ToColoredString("[| Powered by SadConsole |]");
            var logoText2 = new ColorGradient(Color.Magenta, Color.Yellow)
                                .ToColoredString("[| Powered by SadConsole |]");

            var s = new ScreenObject
            {
                // Position = (20, 20)
            };
            ReplaceSC(s);

            var c = new Console(Width, Height, GetRandomBackgroundGlyphs(Width * Height));
            s.Children.Add(c);
            c.Print(2, 2, s.AbsolutePosition.ToString(), Color.White, Color.Black);

            InstructionSet animation = new InstructionSet()

                    // Animation to move the angled gradient spotlight effect
                    .Code(MoveGradient)

                    // Clear the background text so new printing doesn't look bad
                    .Code((host, delta) =>
                    {
                        ((IScreenSurface)host).Surface.Fill(Color.Black, Color.Transparent, 0);
                        return true;
                    })

                    // Draw the SadConsole text at the bottom
                    .InstructConcurrent(
                        new DrawString(logoText)
                        {
                            Position = new Point(26, Height - 3),
                            TotalTimeToPrint = 4f
                        }, 
                        
                        new DrawString(logoText2)
                        {
                            Position = new Point(26, Height - 1),
                            TotalTimeToPrint = 2f
                        }
                    );

            animation.RemoveOnFinished = true;

            c.SadComponents.Add(animation);

            bool MoveGradient(IScreenObject console, TimeSpan delta)
            {
                gradientPositionX += gradientChange;

                if (gradientPositionX > Width + 50)
                {
                    return true;
                }
                /*
                if ( (gradientChange == 1 && gradientPositionX > Width + 50) || (gradientChange == -1 && gradientPositionX < -50) )
                {
                    gradientChange = -gradientChange;
                    angle += angleChange;

                    if (angle == 90 || angle == 45)
                    {
                        angleChange = -angleChange;
                    }
                }
                */

                Color[] colors = new[] { Color.Black, Color.Green, Color.White, Color.Blue, Color.Black };
                float[] colorStops = new[] { 0f, 0.2f, 0.5f, 0.8f, 1f };

                Algorithms.GradientFill(
                    c.FontSize, 
                    new Point(gradientPositionX, 12), 
                    20, 
                    angle, 
                    new Rectangle(0, 0, Width, Height), 
                    new ColorGradient(colors, colorStops), 
                    (f, b, col) => ((IScreenSurface)c).Surface.SetForeground(f, b, col)
                );

                return false;
            }

        }

        // string parser https://sadconsole.com/v8/articles/string-parser.html
        static void InitStringParser()
        {
            var sc = Game.Instance.StartingConsole;

            int row = 1;

            sc.Print(1, row++, new ColoredString("[c:r f:Aqua]Strings with commands embedded"));
            Print();

            row += 2;
            sc.UsePrintProcessor = true;
            sc.Print(1, row++, "[c:r f:Aqua]Strings when run through the processor");
            Print();

            void Print()
            {
                row++;
                sc.Print(1, row++, "Normal [c:r f:Blue][c:r b:Yellow]and colored");
                sc.Print(1, row++, "Normal [c:r f:0,255,0][c:r b:128,128,0,255]and colored");
                sc.Print(1, row++, "Normal [c:r f:0,0,255,128][c:r b:255,255,0]and [c:r f:0,0,255,64]co[c:r f:0,0,255,32]lo[c:r f:0,0,255,16]red");
                sc.Print(1, row++, "[c:r f:red]Colored and [c:r f:x,128,x]adjusted");
                sc.Print(1, row++, "Normal and [c:r f:blue][c:r b:yellow]colored and[c:undo] undo [c:undo]text");
            } 
        }

        // effects and decorators
        static void InitEffectsAndDecorators()
        {
            var sc = Game.Instance.StartingConsole;
            sc.DefaultBackground = Color.AnsiBlackBright;
            sc.Clear();

            // resize the font for better clarity
            sc.FontSize = new Point(20, 40);

            // create a player glyph
            var top = new CellDecorator(Color.AnsiCyan, 0x7e, Mirror.None);
            var bottom = new CellDecorator(Color.AnsiGreen, 0x7e, Mirror.Vertical);
            var left = new CellDecorator(Color.Blue, 0xaa, Mirror.Horizontal);
            var right = new CellDecorator(Color.Red, 0xaa, Mirror.None);
            CellDecorator[] decorators = { top, bottom, left, right };
            var playerGlyph = new ColoredGlyph(Color.White, Color.AnsiBlackBright, 1, Mirror.None, true, decorators);

            // place the glyph on the surface
            sc.SetCellAppearance(2, 4, playerGlyph);

            // different methods of copying appearance
            sc.SetCellAppearance(4, 4, sc[2, 4]);
            sc.SetCellAppearance(6, 4, sc.GetCellAppearance(2, 4));
            sc[2, 4].CopyAppearanceTo(sc[8, 4]);
            sc[10, 4].CopyAppearanceFrom(sc[2, 4]);
            sc[12, 4].CopyAppearanceFrom(sc[2, 4]);

            // apply blink effect
            Blink blink = new();
            sc.SetEffect(2, 4, blink);

            // create a fade effect
            Fade fade = new()
            {
                FadeBackground = true,
                UseCellBackground = false,
                DestinationBackground = new ColorGradient(Color.Green, Color.Red, Color.Blue),
                FadeDuration = 3,
                CloneOnAdd = true,
                Repeat = true,
                AutoReverse = true,
                StartDelay = 1
            };

            // apply fade effect
            sc.SetEffect(4, 4, fade);
            ClearDecorators(sc[4, 4]);

            // apply fade effect to a neighbour
            sc.SetEffect(6, 4, fade);
            ClearDecorators(sc[6, 4]);

            // modify fade on the neighbour
            var clonedFade = sc.GetEffect(6, 4) as Fade;
            clonedFade.UseCellBackground = true;
            clonedFade.DestinationBackground = new ColorGradient(Color.Green, Color.Blue, Color.Red);
            clonedFade.FadeForeground = true;

            // create a blink glyph effect
            BlinkGlyph blinkGlyph = new()
            {
                GlyphIndex = 0x05,
                CloneOnAdd = true,
                StartDelay = 0.5d
            };

            // apply the blink glyph effect
            sc.SetEffect(8, 4, blinkGlyph);
            ClearDecorators(sc[8, 4]);
            
            // create a chain of effects
            EffectsChain effectsChain = new()
            {
                Repeat = true,
                DelayBetweenEffects = 1
            };

            effectsChain.Effects.Add(new Fade
            {
                FadeBackground = true,
                UseCellBackground = true,
                UseCellDestinationReverse = false,
                DestinationBackground = new ColorGradient(Color.White, Color.Green, Color.Yellow),
                FadeDuration = 4,
                Repeat = false,
                AutoReverse = true
            });

            effectsChain.Effects.Add(new Fade
            {
                FadeForeground = true,
                UseCellForeground = false,
                UseCellDestinationReverse = false,
                DestinationForeground = new ColorGradient(Color.Green, Color.AnsiRed),
                FadeDuration = 4,
                Repeat = false,
                AutoReverse = true
            });

            effectsChain.Effects.Add(new Fade
            {
                FadeBackground = true,
                UseCellBackground = true,
                UseCellDestinationReverse = false,
                DestinationBackground = new ColorGradient(Color.White, Color.Blue, Color.Red),
                FadeDuration = 4,
                Repeat = false,
                AutoReverse = true
            });

            effectsChain.Effects.Add(new Fade
            {
                FadeForeground = true,
                UseCellForeground = true,
                UseCellDestinationReverse = false,
                DestinationForeground = new ColorGradient(Color.Green, Color.AnsiBlue),
                FadeDuration = 4,
                Repeat = false,
                AutoReverse = true
            });

            // apply chain
            sc.SetEffect(10, 4, effectsChain);
            ClearDecorators(sc[10, 4]);
            (sc.GetEffect(10, 4) as EffectsChain).Start();

            void ClearDecorators(ColoredGlyph cg)
            {
                cg.Decorators = new CellDecorator[] { };
            }
        }

        // splash screens
        static void InitSplashScreens()
        {
            var sc = Game.Instance.StartingConsole;
            sc.Print(2, 2, "Press either space or shift to see both splash screens.");

            Func<IScreenObject, Keyboard, bool> keyboardHandler = (host, keyboard) =>
            {
                if (host is Console c && keyboard.HasKeysPressed)
                {
                    c.DefaultBackground = RandomColor;
                    c.Clear();
                    c.Children.Clear();
                    if (keyboard.IsKeyPressed(Keys.Space))
                    {
                        c.Children.Add(new SadConsole.SplashScreens.Simple());
                        
                    }
                    else if (keyboard.IsKeyPressed(Keys.LeftShift))
                    {
                        c.Children.Add(new SadConsole.SplashScreens.PCBoot());
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            };

            SadConsole.Quick.Keyboard.WithKeyboard(sc, keyboardHandler);
        }

        // loading an external font from a file
        static void InitLoadingFont()
        {
            // change font in starting console
            var sc = Game.Instance.StartingConsole;
            Game.Instance.LoadFont(@"Fonts/chess.font");
            sc.Font = Game.Instance.Fonts["chess"];

            // create an additional console with a default font
            var c = new Console(30, 5);
            c.DefaultBackground = Color.AnsiCyan;
            c.Clear();

            // print some info on the new console
            c.Print(0, 0, "Number of fonts: " + Game.Instance.Fonts.Count.ToString());
            c.Cursor.Move(0, 1).PrintAppearanceMatchesHost = false;
            foreach (string key in Game.Instance.Fonts.Keys)
            {
                ColoredString txtKey = new(key, RandomColor, c.DefaultBackground);
                ColoredGlyph glyph = new(RandomColor, c.DefaultBackground);
                c.Cursor
                    .NewLine()
                    //.SetPrintAppearance(RandomColor)
                    .SetPrintAppearance(RandomColor.GetDarker(), c.DefaultBackground)
                    //.SetPrintAppearance(glyph)
                    .Print($"    {key}");
            }

            // add the new console to the children of sc
            sc.Children.Add(c);
            c.Position = new Point(40, 2);
            
            // print some glyphs from the new font on the sc
            int x = 0, y = 7, glyphNumber = 48;
            for (int b = 1; b <= 6; b++)
            {
                for (int a = 0; a < 16; a++)
                {
                    sc.SetGlyph(x + a * 2, y + b * 2, glyphNumber++, RandomColor);
                }
            }
        }

        // exercise in moving and resizing of a console and its view with arrow keys
        static void InitMoveAndResizeViewWithArrowKeys()
        {
            var c = new MoveAndResizeScreen();
        }

        // simple movable character
        static void InitMovableCharacter()
        {
            var map = new MapScreen();
        }

        // keyboard and mouse handling via components or events
        static void InitKeyboardAnMouse()
        {
            ScreenObject container = new ScreenObject();
            Game.Instance.Screen = container;
            Game.Instance.DestroyDefaultStartingConsole();

            Console c1 = new Console(30, 15);
            c1.Position = new Point(10, 1);
            c1.DefaultBackground = Color.AnsiGreen;
            c1.Clear();
            c1.IsFocused = true;
            c1.Cursor.Position = new Point(15, 7);
            // c1.Cursor.IsEnabled = true;
            c1.Cursor.IsVisible = true;
            c1.Cursor.MouseClickReposition = true;

            c1.SadComponents.Add(new RandomBackgroundKeyboardComponent());
            c1.MouseMove += OnMouseMove;
            c1.MouseButtonClicked += OnMouseButtonClicked;
            c1.MouseExit += OnMouseExit;
            c1.MouseEnter += OnMouseEnter;

            var c2 = new Console(30, 15);
            c2.Position = new Point(28, 13);
            c2.DefaultBackground = Color.AnsiCyan;
            c2.Clear();

            container.Children.Add(c2);
            container.Children.Add(c1);

            void OnMouseEnter(object sender, MouseScreenObjectState mouseState)
            {
                var c = sender as Console;
                if (c is not null)
                {
                    c.Cursor.IsVisible = true;
                    c.Cursor.Position = new Point(15, 7);
                }
            }

            void OnMouseExit(object sender, MouseScreenObjectState mouseState)
            {
                var c = sender as Console;
                if (c is not null)
                {
                    c.Cursor.IsVisible = false;
                    c.Clear();
                }
            }

            void OnMouseMove(object sender, MouseScreenObjectState mouseState)
            {
                var c = sender as Console;
                if (c is not null)
                {
                    c.Print(1, 1, $"Mouse position: {mouseState.CellPosition}  ");
                    /*
                    if (mouseState.Mouse.LeftButtonDown)
                        c.Print(1, 2, $"Left button is down");
                    else
                        c.Print(1, 2, $"                   ");
                    */
                }
            }

            void OnMouseButtonClicked(object sender, MouseScreenObjectState mouseState)
            {
                var c = sender as Console;
                if (c is not null)
                {
                    if (mouseState.Mouse.LeftButtonDown)
                        c.Print(1, 2, $"Left button is down");
                    else
                        c.Print(1, 2, $"                   ");
                }
            }
        }

        // creating a container for multiple consoles
        static void InitOverlappingConsoles()
        {
            ScreenObject container = new();
            ReplaceSC(container);

            // First console
            Console console1 = new(60, 14);
            console1.Position = new Point(3, 2);
            console1.DefaultBackground = Color.AnsiCyan;
            console1.Clear();
            console1.Print(1, 1, "Type on me!");
            console1.Cursor.Position = new Point(1, 2);
            console1.Cursor.IsEnabled = true;
            console1.Cursor.IsVisible = true;
            console1.Cursor.MouseClickReposition = true;
            console1.IsFocused = true;

            console1.FocusOnMouseClick = true;
            console1.MoveToFrontOnMouseClick = true;

            // Add a child surface
            ScreenSurface surfaceObject = new ScreenSurface(5, 3);
            surfaceObject.Surface.FillWithRandomGarbage(surfaceObject.Font);
            surfaceObject.Position = console1.Area.Center - (surfaceObject.Surface.Area.Size / 2);
            surfaceObject.UseMouse = false;

            console1.Children.Add(surfaceObject);

            // Second console
            SadConsole.Console console2 = new SadConsole.Console(58, 12);
            console2.Position = new Point(19, 11);
            console2.DefaultBackground = Color.AnsiRed;
            console2.Clear();
            console2.Print(1, 1, "Type on me!");
            console2.Cursor.Position = new Point(1, 2);
            console2.Cursor.IsEnabled = true;
            console2.Cursor.IsVisible = true;
            console2.FocusOnMouseClick = true;
            console2.MoveToFrontOnMouseClick = true;

            // container.Children.Add(console2);
            // container.Children.MoveToBottom(console2);

            // order matters
            container.Children.Add(console2);
            container.Children.Add(console1);
        }

        // http://sadconsole.com/v9/articles/tutorials/getting-started/part-2-cursor-parents.html
        static void InitPlayingWithCursor()
        {
            var cursor = Game.Instance.StartingConsole.Cursor;
            cursor.PrintAppearanceMatchesHost = false;
            cursor.Move(0, 21)
                .Print("Kato is my favorite dog")
                .NewLine()
                .SetPrintAppearance(RandomColor)
                .Print("No, Birdie is my favorite dog");

            cursor.IsVisible = true;
            cursor.IsEnabled = true;
        }

        // http://sadconsole.com/v9/articles/tutorials/getting-started/part-1-drawing.html
        static void InitBasicDrawing()
        {
            Console sc = Game.Instance.StartingConsole;
            ColoredGlyph glyph = new ColoredGlyph(Color.Violet, Color.Black, 177);
            ColoredGlyph standardGlyph = new ColoredGlyph(Color.White, Color.Black);
            Rectangle rectangle = new Rectangle(3, 3, 23, 3);
            Rectangle rectangle2 = new Rectangle(3, 7, 23, 3);
            Rectangle rectangle3 = new Rectangle(3, 11, 23, 3);

            // sc.FillWithRandomGarbage(SadConsole.Game.Instance.StartingConsole.Font);
            sc.Fill(rectangle, Color.Violet, Color.Black, 0);
            sc.Print(4, 4, "Hello from SadConsole", Mirror.None);

            // simple box
            sc.DrawBox(rectangle, ShapeParameters.CreateBorder(glyph));

            // thin line border box
            sc.DrawBox(rectangle2, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, glyph));

            // thick line border box
            sc.DrawBox(rectangle3, ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThick, glyph));

            // circle 
            sc.DrawCircle(new Rectangle(28, 5, 16, 10), ShapeParameters.CreateFilled(glyph, standardGlyph));

            // line 
            sc.DrawLine(new Point(60, 5), new Point(66, 20), '$', Color.AnsiBlue, Color.AnsiBlueBright);

            // manipulate glyphs
            sc.SetForeground(15, 4, Color.DarkGreen);
            sc.SetBackground(18, 4, Color.DarkCyan);
            sc.SetGlyph(4, 4, 64);
            sc.SetMirror(10, 4, Mirror.Vertical);
        }

        #endregion

        #region Helper Functions

        // returns a random color
        public static Color RandomColor => Color.White.GetRandomColor(Game.Instance.Random);

        // replaces the starting console with a screenobject s
        public static void ReplaceSC(IScreenObject s)
        {
            Game.Instance.Screen = s;
            Game.Instance.DestroyDefaultStartingConsole();
        }

        // returns an array with a seed of characters for a background
        public static ColoredGlyph[] GetRandomBackgroundGlyphs(int count)
        {
            var cg = new List<ColoredGlyph>();
            for (int i = 0; i < count; i++)
            {
                cg.Add(new ColoredGlyph(RandomColor, RandomColor, 177 /*Game.Instance.Random.Next(176, 178)*/));
            }
            return cg.ToArray();
        }

        static void Test(IScreenObject s)
        {
            ReplaceSC(s);
        }

        #endregion
    }
}